using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Visitor;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Linq.Sparql
{
    /// <summary>Povides a SPARQL query parsing mechanism.</summary>
    public class GenericSparqlQueryVisitor : QueryVisitorBase
    {
        #region Fields
        private static readonly IDictionary<MethodNames, string> MethodNameMap = new Dictionary<MethodNames, string>();
        private readonly Stack<StrongEntityAccessor> _currentEntityAccessor = new Stack<StrongEntityAccessor>();
        private StringBuilder _commandText;
        private string _metaGraphVariableName;
        private string _entityVariableName;
        private string _subjectVariableName;
        private string _predicateVariableName;
        private string _objectVariableName;
        private string _scalarVariableName;
        private bool _cancelLast = false;
        private StrongEntityAccessor _entityAccessorToExpand = null;
        private IDictionary<Identifier, string> _variableNameOverride = new Dictionary<Identifier, string>();
        private IList<IQueryComponent> _supressedComponents = new List<IQueryComponent>();
        private IList<IQueryComponent> _injectedComponents = new List<IQueryComponent>();
        private VisitedComponentCollection _visitedComponents;
        #endregion

        #region Constructors
        static GenericSparqlQueryVisitor()
        {
            MethodNameMap[MethodNames.Any] = "EXISTS";
            MethodNameMap[MethodNames.StartsWith] = "STRSTARTS";
            MethodNameMap[MethodNames.EndsWith] = "STRENDS";
            MethodNameMap[MethodNames.Length] = "STRLEN";
            MethodNameMap[MethodNames.ToUpper] = "UCASE";
            MethodNameMap[MethodNames.ToLower] = "LCASE";
            MethodNameMap[MethodNames.Substring] = "SUBSTR";
            MethodNameMap[MethodNames.Ceiling] = "CEIL";
            MethodNameMap[MethodNames.Hour] = "HOURS";
            MethodNameMap[MethodNames.Minute] = "MINUTES";
            MethodNameMap[MethodNames.Second] = "SECONDS";
            MethodNameMap[MethodNames.Milisecond] = "SECONDS";
            MethodNameMap[MethodNames.RandomFloat] = "RAND";
            MethodNameMap[MethodNames.RandomInt] = "RAND";
        }
        #endregion

        #region Properties
        /// <summary>Gets a command text string.</summary>
        public string CommandText { get { return (_commandText != null ? _commandText.ToString() : String.Empty); } }

        /// <summary>
        /// Gets the SPARQL query's variables.
        /// </summary>
        public SparqlQueryVariables Variables
        {
            get
            {
                return new SparqlQueryVariables(
                    _entityVariableName,
                    _subjectVariableName,
                    _predicateVariableName,
                    _objectVariableName,
                    _metaGraphVariableName,
                    _scalarVariableName);
            }
        }

        /// <summary>Gets the <see cref="StringBuilder" /> used to create the query command text.</summary>
        protected StringBuilder CommandTextBuilder { get { return _commandText; } }

        /// <summary>Gets the current <see cref="StrongEntityAccessor" />.</summary>
        protected StrongEntityAccessor CurrentEntityAccessor { get { return (_currentEntityAccessor.Count > 0 ? _currentEntityAccessor.Peek() : null); } }
        #endregion

        #region Public methods
        /// <summary>Visit a query component.</summary>
        /// <param name="component">Component to be visited.</param>
        public override void VisitComponent(IQueryComponent component)
        {
            if (!_supressedComponents.Contains(component))
            {
                base.VisitComponent(component);
            }
        }

        /// <summary>Visit a query.</summary>
        /// <param name="query">Query to be visited.</param>
        public override void VisitQuery(Query query)
        {
            InitQuery(query.IsSubQuery ? null : query.Prefixes);
            BeginQuery(query.IsSubQuery, query.QueryForm, query.Select, query.CreateVariableName);
            _entityAccessorToExpand = ((query.Limit >= 0) || (query.Offset >= 0) || (query.OrderBy.Count > 0) ? (StrongEntityAccessor)query.Elements.LastOrDefault(item => item is StrongEntityAccessor) : null);
            foreach (QueryElement element in query.Elements)
            {
                VisitComponent(element);
            }

            EndQuery(query.IsSubQuery, (_entityAccessorToExpand != null ? query.OrderBy : null));
        }
        #endregion

        #region Protected methods
        /// <summary>Visit a function call.</summary>
        /// <param name="call">Function call to be visited.</param>
        protected override void VisitCall(Call call)
        {
            string functionName = call.Member.ToString().ToUpper();
            string openingBracket = "(";
            string closingBracket = ")";
            string separator = String.Empty;
            string targetAccessor = String.Empty;
            ICollection<IExpression> arguments = call.Arguments;
            IExpression target = null;
            switch (call.Member)
            {
                case MethodNames.Any:
                    openingBracket = closingBracket = " ";
                    goto default;
                case MethodNames.In:
                    targetAccessor = separator = " ";
                    target = (call.Arguments.Count > 0 ? call.Arguments.First() : null);
                    arguments = (call.Arguments.Count > 1 ? call.Arguments.Skip(1).ToList() : new List<IExpression>());
                    goto default;
                case MethodNames.RandomInt:
                    closingBracket += "*1000000000";
                    goto default;
                default:
                    if (MethodNameMap.ContainsKey(call.Member))
                    {
                        functionName = MethodNameMap[call.Member];
                    }

                    break;
            }

            if (target != null)
            {
                VisitComponent(target);
            }

            _commandText.AppendFormat("{0}{1}{2}{3}", targetAccessor, functionName, separator, openingBracket);
            int index = 0;
            foreach (IExpression argument in arguments)
            {
                VisitComponent(argument);
                if (index < arguments.Count - 1)
                {
                    _commandText.Append(",");
                }

                index++;
            }

            _commandText.AppendFormat("{0}", closingBracket);
        }

        /// <summary>Visit an unary operator.</summary>
        /// <param name="unaryOperator">Unary operator to be visited.</param>
        protected override void VisitUnaryOperator(UnaryOperator unaryOperator)
        {
            string operatorString;
            switch (unaryOperator.Member)
            {
                case MethodNames.Not:
                    operatorString = "!";
                    break;
                default:
                    throw new NotImplementedException(String.Format("Unary operator '{0}' is not supported.", unaryOperator.Member));
            }

            _commandText.Append(operatorString);
            VisitComponent(unaryOperator.Operand);
        }

        /// <summary>Visit a binary operator.</summary>
        /// <param name="binaryOperator">Binary operator to be visited.</param>
        protected override void VisitBinaryOperator(BinaryOperator binaryOperator)
        {
            if (IsBinaryOperatorComplexEntityContrain(binaryOperator))
            {
                VisitComplexEntityContrain(binaryOperator);
            }
            else if (IsBinaryOperatorEntityIsNullCheck(binaryOperator))
            {
                VisitEntityIsNullCheck(binaryOperator);
            }
            else
            {
                string operatorString;
                switch (binaryOperator.Member)
                {
                    case MethodNames.Add:
                        operatorString = "+";
                        break;
                    case MethodNames.Substract:
                        operatorString = "-";
                        break;
                    case MethodNames.Multiply:
                        operatorString = "*";
                        break;
                    case MethodNames.Divide:
                        operatorString = "/";
                        break;
                    case MethodNames.Equal:
                        operatorString = "=";
                        break;
                    case MethodNames.GreaterThan:
                        operatorString = ">";
                        break;
                    case MethodNames.GreaterThanOrEqual:
                        operatorString = ">=";
                        break;
                    case MethodNames.LessThan:
                        operatorString = "<";
                        break;
                    case MethodNames.LessThanOrEqual:
                        operatorString = "<=";
                        break;
                    case MethodNames.NotEqual:
                        operatorString = "!=";
                        break;
                    case MethodNames.Or:
                        operatorString = "||";
                        break;
                    case MethodNames.And:
                        operatorString = "&&";
                        break;
                    default:
                        throw new NotImplementedException(String.Format("Binary operator '{0}' is not supported.", binaryOperator.Member));
                }

                VisitComponent(binaryOperator.LeftOperand);
                bool leftOperandCanceled = _cancelLast;
                _cancelLast = false;
                int startIndex = _commandText.Length;
                if (!leftOperandCanceled)
                {
                    _commandText.AppendFormat(" {0} ", operatorString);
                }

                VisitComponent(binaryOperator.RightOperand);
                bool rightOperandCanceled = _cancelLast;
                _cancelLast = false;
                int length = _commandText.Length - startIndex;
                if (rightOperandCanceled)
                {
                    _commandText = _commandText.Remove(startIndex, length);
                }

                _cancelLast = leftOperandCanceled && rightOperandCanceled;
            }
        }

        /// <summary>Visit an entity constrain.</summary>
        /// <param name="entityConstrain">Entity constrain to be visited.</param>
        protected override void VisitEntityConstrain(EntityConstrain entityConstrain)
        {
            int startIndex = _commandText.Length;
            VisitComponent(_currentEntityAccessor.Peek().About);
            _commandText.Append(" ");
            VisitComponent(entityConstrain.Predicate);
            _commandText.Append(" ");
            VisitComponent(entityConstrain.Value);
            _commandText.Append(" . ");
            int length = _commandText.Length - startIndex;
            if (!_visitedComponents.Contains(entityConstrain))
            {
                _visitedComponents.Add(entityConstrain, startIndex, length);
            }
        }

        /// <summary>Visit an entity type constrain.</summary>
        /// <param name="entityTypeConstrain">Entity type constrain to be visited.</param>
        protected override void VisitEntityTypeConstrain(EntityTypeConstrain entityTypeConstrain)
        {
            if (entityTypeConstrain.InheritedTypes.Any())
            {
                if (_entityAccessorToExpand != null)
                {
                    VisitComponent(_entityAccessorToExpand.About);
                    _commandText.AppendFormat(" <{0}> ?{1}_type . ", Rdf.type, (_variableNameOverride.ContainsKey(_entityAccessorToExpand.About) ? _variableNameOverride[_entityAccessorToExpand.About] : _entityAccessorToExpand.About.Name));
                }

                _commandText.Append("FILTER ( EXISTS { ");
                _commandText.AppendFormat("?{0} ", _currentEntityAccessor.Peek().About.Name);
                VisitComponent(entityTypeConstrain.Predicate);
                _commandText.Append(" ");
                VisitComponent(entityTypeConstrain.Value);
                _commandText.Append(" . } ");
                foreach (Literal inheritedType in entityTypeConstrain.InheritedTypes)
                {
                    _commandText.Append("|| EXISTS { ");
                    _commandText.AppendFormat("?{0} ", _currentEntityAccessor.Peek().About.Name);
                    VisitComponent(entityTypeConstrain.Predicate);
                    _commandText.Append(" ");
                    VisitComponent(inheritedType);
                    _commandText.Append(" . } ");
                }

                _commandText.Append(") ");
            }
            else
            {
                VisitEntityConstrain(entityTypeConstrain);
            }
        }

        /// <summary>Visit an unbound constrain.</summary>
        /// <param name="unboundConstrain">Unbound constrain to be visited.</param>
        protected override void VisitUnboundConstrain(UnboundConstrain unboundConstrain)
        {
            VisitComponent(unboundConstrain.Subject);
            _commandText.Append(" ");
            VisitComponent(unboundConstrain.Predicate);
            _commandText.Append(" ");
            VisitComponent(unboundConstrain.Value);
            _commandText.Append(" . ");
        }

        /// <summary>Visit a literal.</summary>
        /// <param name="literal">Literal to be visited.</param>
        protected override void VisitLiteral(Literal literal)
        {
            if (literal.Value == null)
            {
                VisitNullLiteral(literal.ValueType);
            }
            else
            {
                string valueString;
                switch (literal.ValueType.FullName)
                {
                    default:
                        if (literal.Value is EntityId)
                        {
                            valueString = String.Format("<{0}>", literal.Value);
                        }
                        else
                        {
                            valueString = String.Format("\"{0}\"", literal.Value);
                        }

                        break;
                    case "System.Byte":
                    case "System.SByte":
                    case "System.Int16":
                    case "System.UInt16":
                    case "System.Int32":
                    case "System.UInt32":
                    case "System.Int64":
                    case "System.UInt64":
                        valueString = literal.Value.ToString();
                        break;
                    case "System.Char":
                    case "System.String":
                        valueString = String.Format("\"{0}\"^^xsd:string", literal.Value);
                        break;
                    case "System.Single":
                    case "System.Double":
                    case "System.Decimal":
                        valueString = String.Format(CultureInfo.InvariantCulture, "{0}", literal.Value);
                        break;
                    case "System.DateTime":
                        valueString = String.Format(CultureInfo.InvariantCulture, "\"{0:s}\"^^xsd:dateTime", literal.Value);
                        break;
                    case "System.Uri":
                        valueString = String.Format("<{0}>", literal.Value);
                        break;
                }

                _commandText.Append(valueString);
            }
        }

        /// <summary>Visit a list.</summary>
        /// <param name="list">List to be visited.</param>
        protected override void VisitList(List list)
        {
            int index = 0;
            foreach (Literal literal in list.Values)
            {
                VisitComponent(literal);
                if (index < list.Values.Count - 1)
                {
                    _commandText.Append(",");
                }

                index++;
            }
        }

        /// <summary>Visit an alias.</summary>
        /// <param name="alias">Alias to be visited.</param>
        protected override void VisitAlias(Alias alias)
        {
            VisitComponent(alias.Component);
            _commandText.Append(" AS ");
            VisitComponent(alias.Name);
        }

        /// <summary>Visit a prefix.</summary>
        /// <param name="prefix">Prefix to be visited.</param>
        protected override void VisitPrefix(Prefix prefix)
        {
            _commandText.AppendFormat("PREFIX {0}: <{1}> ", prefix.NamespacePrefix, prefix.NamespaceUri);
        }

        /// <summary>Visit a identifier.</summary>
        /// <param name="identifier">Identifier to be visited.</param>
        protected override void VisitIdentifier(Identifier identifier)
        {
            _commandText.AppendFormat("?{0}", (_variableNameOverride.ContainsKey(identifier) ? _variableNameOverride[identifier] : identifier.Name));
        }

        /// <summary>Visit a filter.</summary>
        /// <param name="filter">Filter to be visited.</param>
        protected override void VisitFilter(Filter filter)
        {
            int startIndex = _commandText.Length;
            _commandText.Append("FILTER (");
            VisitComponent(filter.Expression);
            _commandText.Append(") ");
            int length = _commandText.Length - startIndex;
            if (_cancelLast)
            {
                _commandText.Remove(startIndex, length);
            }

            _cancelLast = false;
        }

        /// <summary>Visit an identifier entity accessor.</summary>
        /// <param name="entityAccessor">Identifier entity accessor to be visited.</param>
        protected override void VisitIdentifierEntityAccessor(IdentifierEntityAccessor entityAccessor)
        {
            Call predicateBind = new Call(MethodNames.Bind);
            predicateBind.Arguments.Add(new Alias(new Literal(Rdf.predicate), new Identifier(entityAccessor.About.Name + "_Predicate")));
            Call objectBind = new Call(MethodNames.Bind);
            objectBind.Arguments.Add(new Alias(new Literal(Rdf.@object), new Identifier(entityAccessor.About.Name + "_Object")));
            _injectedComponents.Add(predicateBind);
            _injectedComponents.Add(objectBind);
            VisitStrongEntityAccessor(entityAccessor.EntityAccessor);
            _injectedComponents.Clear();
        }

        /// <summary>Visit an unspecified entity accessor.</summary>
        /// <param name="entityAccessor">Unspecified entity accessor to be visited.</param>
        protected override void VisitUnspecifiedEntityAccessor(UnspecifiedEntityAccessor entityAccessor)
        {
            _currentEntityAccessor.Push(entityAccessor);
            _commandText.Append("{ ");
            VisitStrongEntityAccessor(entityAccessor.EntityAccessor);
            _commandText.Append("} UNION { ");
            foreach (IQueryComponent component in entityAccessor.Elements)
            {
                _supressedComponents.Add(component);
            }

            VisitStrongEntityAccessor(entityAccessor.EntityAccessor);
            _supressedComponents.Clear();
            _commandText.AppendFormat("GRAPH ?G{0} {{ ", entityAccessor.About.Name);
            foreach (QueryElement element in entityAccessor.Elements)
            {
                VisitComponent(element);
            }

            _commandText.Append("} ");
            _commandText.AppendFormat("GRAPH <{0}> {{ ?G{1} <http://xmlns.com/foaf/0.1/primaryTopic> ", MetaGraphUri, entityAccessor.About.Name);
            VisitComponent(entityAccessor.About);
            _commandText.Append(" . } } ");
            _currentEntityAccessor.Pop();
        }

        /// <summary>Visit a strong entity accessor.</summary>
        /// <param name="entityAccessor">Strong entity accessor to be visited.</param>
        protected override void VisitStrongEntityAccessor(StrongEntityAccessor entityAccessor)
        {
            int startIndex = _commandText.Length;
            _currentEntityAccessor.Push(entityAccessor);
            _commandText.AppendFormat("GRAPH ?G{0} {{ ", entityAccessor.About.Name);
            foreach (QueryElement element in entityAccessor.Elements)
            {
                VisitComponent(element);
            }

            foreach (IQueryComponent element in _injectedComponents)
            {
                VisitComponent(element);
                if (element is Call)
                {
                    _commandText.Append(" ");
                }
            }

            if ((_entityAccessorToExpand != null) && (_entityAccessorToExpand.Equals(entityAccessor)))
            {
                int currentStartIndex = _commandText.Length;
                _entityAccessorToExpand.FindAllComponents<Identifier>().Select(item => _variableNameOverride[item] = item.Name + "_sub").ToList();
                _commandText.Append("{ SELECT DISTINCT ");
                VisitComponent(_entityAccessorToExpand.About);
                _commandText.AppendFormat(" WHERE {{ GRAPH ?G{0} {{ ", _variableNameOverride[_entityAccessorToExpand.About]);
                foreach (QueryElement element in entityAccessor.Elements.SkipWhile(item => item is UnboundConstrain))
                {
                    VisitComponent(element);
                }

                _commandText.AppendFormat("}} GRAPH <{0}> {{ ?G{1} <{2}> ?{1} . }} }} ", MetaGraphUri, _variableNameOverride[_entityAccessorToExpand.About], Foaf.primaryTopic);
                VisitQueryResultModifiers(_entityAccessorToExpand.OwnerQuery.OrderBy, _entityAccessorToExpand.OwnerQuery.Offset, _entityAccessorToExpand.OwnerQuery.Limit);
                _commandText.Append("} FILTER (");
                VisitComponent(_entityAccessorToExpand.About);
                _variableNameOverride.Clear();
                _commandText.Append("=");
                VisitComponent(_entityAccessorToExpand.About);
                _commandText.Append(") ");

                int currentLength = _commandText.Length - currentStartIndex;
                string expansionText = _commandText.ToString().Substring(currentStartIndex, currentLength);
                _commandText = _commandText.Remove(currentStartIndex, currentLength).Insert(startIndex, expansionText);
                _visitedComponents.Update(startIndex, currentLength);
            }

            _commandText.AppendFormat("}} GRAPH <{0}> {{ ?G{1} <{2}> ?{1} . }} ", MetaGraphUri, entityAccessor.About.Name, Foaf.primaryTopic);
            _currentEntityAccessor.Pop();
        }

        /// <summary>Visit an optional patterns.</summary>
        /// <param name="optionalPattern">Optional patterns accessor to be visited.</param>
        protected override void VisitOptionalPattern(OptionalPattern optionalPattern)
        {
            _commandText.Append("OPTIONAL { ");
            foreach (EntityConstrain pattern in optionalPattern.Patterns)
            {
                VisitComponent(pattern);
            }

            _commandText.Append(" } ");
        }

        /// <summary>Visits a dicionary of query result modifiers with optional offset and limit.</summary>
        /// <param name="orderByExpressions">Dictionary of result modiefiers.</param>
        /// <param name="offset">Offset in the resultset.</param>
        /// <param name="limit">Limit of the resultset.</param>
        protected virtual void VisitQueryResultModifiers(IDictionary<IExpression, bool> orderByExpressions, int offset, int limit)
        {
            if (orderByExpressions.Count > 0)
            {
                VisitOrderBy(orderByExpressions);
            }

            if (_entityAccessorToExpand.OwnerQuery.Offset >= 0)
            {
                _commandText.AppendFormat("OFFSET {0} ", _entityAccessorToExpand.OwnerQuery.Offset);
            }

            if (_entityAccessorToExpand.OwnerQuery.Limit >= 0)
            {
                _commandText.AppendFormat("LIMIT {0} ", _entityAccessorToExpand.OwnerQuery.Limit);
            }
        }
        #endregion

        #region Private methods
        private void VisitComplexEntityContrain(BinaryOperator binaryOperator)
        {
            string operatorString;
            switch (binaryOperator.Member)
            {
                case MethodNames.Or:
                    operatorString = "||";
                    break;
                case MethodNames.And:
                    operatorString = "&&";
                    break;
                default:
                    throw new NotImplementedException(String.Format("Binary operator '{0}' is not supported.", binaryOperator.Member));
            }

            switch (operatorString)
            {
                case "||":
                    {
                        _commandText.Append("EXISTS { ");
                        VisitComponent(binaryOperator.LeftOperand);
                        _commandText.Append(" } || EXISTS { ");
                        VisitComponent(binaryOperator.RightOperand);
                        _commandText.Append("} ");
                        break;
                    }

                case "&&":
                    {
                        _commandText.Append("{ ");
                        VisitComponent(binaryOperator.LeftOperand);
                        VisitComponent(binaryOperator.RightOperand);
                        _commandText.Append("} ");
                        break;
                    }
            }
        }

        private void VisitEntityIsNullCheck(BinaryOperator binaryOperator)
        {
            if (binaryOperator.Member == MethodNames.Equal)
            {
                EntityConstrain entityConstrain;
                _commandText.Append("NOT EXISTS {");
                VisitComponent(_currentEntityAccessor.Peek().About);
                _commandText.Append(" ");
                if ((binaryOperator.RightOperand is Literal) && (((Literal)binaryOperator.RightOperand).Value == null))
                {
                    entityConstrain = _currentEntityAccessor.Peek().FindAllComponents<EntityConstrain>().Where(item => item.Value == binaryOperator.LeftOperand).First();
                    VisitComponent(entityConstrain.Predicate);
                    _commandText.Append(" ");
                    VisitComponent(binaryOperator.LeftOperand);
                }
                else
                {
                    entityConstrain = _currentEntityAccessor.Peek().FindAllComponents<EntityConstrain>().Where(item => item.Value == binaryOperator.RightOperand).First();
                    VisitComponent(entityConstrain.Predicate);
                    _commandText.Append(" ");
                    VisitComponent(binaryOperator.RightOperand);
                }

                _commandText.Append("}");
                if (_currentEntityAccessor.Peek().Elements.IndexOf(entityConstrain) != -1)
                {
                    _visitedComponents.Remove(entityConstrain);
                }
            }
            else
            {
                _cancelLast = true;
            }
        }

        private bool IsBinaryOperatorComplexEntityContrain(BinaryOperator binaryOperator)
        {
            return ((binaryOperator.Member == MethodNames.Or) || (binaryOperator.Member == MethodNames.And)) &&
                ((binaryOperator.LeftOperand is EntityConstrain) || (binaryOperator.RightOperand is EntityConstrain) ||
                ((binaryOperator.LeftOperand is BinaryOperator) && (IsBinaryOperatorComplexEntityContrain((BinaryOperator)binaryOperator.LeftOperand))) ||
                ((binaryOperator.RightOperand is BinaryOperator) && (IsBinaryOperatorComplexEntityContrain((BinaryOperator)binaryOperator.RightOperand))));
        }

        private bool IsBinaryOperatorEntityIsNullCheck(BinaryOperator binaryOperator)
        {
            return ((binaryOperator.Member == MethodNames.Equal) || (binaryOperator.Member == MethodNames.NotEqual)) &&
                (((binaryOperator.RightOperand is Literal) && (((Literal)binaryOperator.RightOperand).Value == null)) ||
                ((binaryOperator.LeftOperand is Literal) && (((Literal)binaryOperator.LeftOperand).Value == null)));
        }

        private void ProcessSelectable(bool isSubQuery, ISelectableQueryComponent expression, Func<string, string> createVariableName)
        {
            if (!isSubQuery)
            {
                if (expression is UnspecifiedEntityAccessor)
                {
                    if ((_metaGraphVariableName == null) && (_entityVariableName == null))
                    {
                        _metaGraphVariableName = "graph";
                        _entityVariableName = ((StrongEntityAccessor)expression).About.Name;
                    }

                    _commandText.AppendFormat(
                        "IF(BOUND(?G{0}),?G{0},?G{1}) AS ?{2} ",
                        ((UnspecifiedEntityAccessor)expression).About.Name,
                        ((UnspecifiedEntityAccessor)expression).EntityAccessor.About.Name,
                        _metaGraphVariableName);
                }
                else if (expression is IdentifierEntityAccessor)
                {
                    if ((_metaGraphVariableName == null) && (_entityVariableName == null))
                    {
                        _metaGraphVariableName = String.Format("G{0}", ((StrongEntityAccessor)expression).About.Name);
                        _entityVariableName = ((StrongEntityAccessor)expression).About.Name + "_Distinct";
                    }

                    _subjectVariableName = ((StrongEntityAccessor)expression).About.Name;
                    _predicateVariableName = ((StrongEntityAccessor)expression).About.Name + "_Predicate";
                    _objectVariableName = ((StrongEntityAccessor)expression).About.Name + "_Object";
                    _commandText.AppendFormat(
                        "DISTINCT(?{2}) AS ?{1} ?{0} ?{3} ?{4} ",
                        _metaGraphVariableName,
                        _entityVariableName,
                        ((StrongEntityAccessor)expression).About.Name,
                        _predicateVariableName,
                        _objectVariableName);
                }
                else if (expression is StrongEntityAccessor)
                {
                    if ((_metaGraphVariableName == null) && (_entityVariableName == null))
                    {
                        _metaGraphVariableName = String.Format("G{0}", _entityVariableName = ((StrongEntityAccessor)expression).About.Name);
                    }

                    _commandText.AppendFormat("?{0} ", _metaGraphVariableName);
                }
                else if (expression is UnboundConstrain)
                {
                    UnboundConstrain unboundConstrain = (UnboundConstrain)expression;
                    if ((unboundConstrain.Subject is Identifier) && (unboundConstrain.Predicate is Identifier) && (unboundConstrain.Value is Identifier))
                    {
                        _subjectVariableName = ((Identifier)unboundConstrain.Subject).Name;
                        _predicateVariableName = ((Identifier)unboundConstrain.Predicate).Name;
                        _objectVariableName = ((Identifier)unboundConstrain.Value).Name;
                    }
                }
                else if (expression is Call)
                {
                    if (_scalarVariableName == null)
                    {
                        _scalarVariableName = createVariableName(((Call)expression).Member.ToString());
                    }
                }
                else if (expression is Alias)
                {
                    Alias alias = (Alias)expression;
                    if ((_scalarVariableName == null) && (alias.Name != null))
                    {
                        _scalarVariableName = alias.Name.Name;
                    }
                }
            }

            foreach (IExpression selectableExpression in expression.Expressions)
            {
                VisitComponent(selectableExpression);
                _commandText.Append(" ");
            }
        }

        private void InitQuery(IEnumerable<Prefix> prefixes)
        {
            if (_commandText == null)
            {
                _visitedComponents = new VisitedComponentCollection(_commandText = new StringBuilder(1024));
            }

            if (prefixes == null)
            {
                _commandText.Append("{ ");
            }
            else
            {
                VisitComponent(new Prefix("xsd", new Uri(Vocabularies.Xsd.BaseUri)));

                foreach (Prefix prefix in prefixes)
                {
                    VisitComponent(prefix);
                }
            }
        }

        private void BeginQuery(bool isSubQuery, QueryForms queryForm, IList<ISelectableQueryComponent> select, Func<string, string> createVariableName)
        {
            _commandText.AppendFormat("{0} ", queryForm.ToString().ToUpper());
            if (queryForm == QueryForms.Select)
            {
                if (select.Count > 0)
                {
                    foreach (ISelectableQueryComponent expression in select)
                    {
                        ProcessSelectable(isSubQuery, expression, createVariableName);
                    }
                }
                else
                {
                    _commandText.Append("* ");
                }
            }

            if (queryForm != QueryForms.Ask)
            {
                _commandText.Append("WHERE ");
            }

            _commandText.Append("{ ");
        }

        private void VisitOrderBy(IDictionary<IExpression, bool> orderByExpressions)
        {
            if (orderByExpressions.Any())
            {
                _commandText.Append("ORDER BY ");
                foreach (KeyValuePair<IExpression, bool> orderBy in orderByExpressions)
                {
                    _commandText.Append(orderBy.Value ? "DESC(" : System.String.Empty);
                    VisitComponent(orderBy.Key);
                    _commandText.Append(orderBy.Value ? ") " : " ");
                }
            }
        }

        private void EndQuery(bool isSubQuery, IDictionary<IExpression, bool> orderByExpressions)
        {
            _commandText.Append("} ");
            if (orderByExpressions != null)
            {
                VisitOrderBy(orderByExpressions);
            }

            if (isSubQuery)
            {
                _commandText.Append("} ");
            }
        }

        private void VisitNullLiteral(Type literalType)
        {
            throw new NotSupportedException("Null literals are not supported in SPARQL.");
        }
        #endregion
    }
}