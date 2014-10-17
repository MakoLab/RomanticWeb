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
    /// <summary>Provides a SPARQL query parsing mechanism.</summary>
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
        private string _ownerVariableName;
        private string _scalarVariableName;
        private bool _cancelLast = false;
        private StrongEntityAccessor _entityAccessorToExpand = null;
        private IDictionary<Identifier, string> _variableNameOverride = new Dictionary<Identifier, string>();
        private IList<IQueryComponent> _supressedComponents = new List<IQueryComponent>();
        private IList<IQueryComponent> _injectedComponents = new List<IQueryComponent>();
        private VisitedComponentCollection _visitedComponents;
        private string _indentation = System.String.Empty;
        private Action<StrongEntityAccessor> _currentStrongEntityAccessorVisitDelegate;
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
            MethodNameMap[MethodNames.String] = "STR";
        }

        /// <summary>Creates an instance of the <see cref="GenericSparqlQueryVisitor"/></summary>
        public GenericSparqlQueryVisitor()
        {
            _currentStrongEntityAccessorVisitDelegate = VisitStrongEntityAccessorInternal;
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
                    _ownerVariableName,
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
            BeginQuery(query.IsSubQuery, query.QueryForm, query.Select, query.CreateVariableName, query.Elements.OfType<StrongEntityAccessor>().FirstOrDefault());
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
            _commandText.Append(_indentation);
            VisitComponent(_currentEntityAccessor.Peek().About);
            _commandText.Append(" ");
            VisitComponent(entityConstrain.Predicate);
            _commandText.Append(" ");
            VisitComponent(entityConstrain.Value);
            _commandText.Append(" . ");
            _commandText.AppendLine();
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
                    _commandText.Append(_indentation);
                    VisitComponent(_entityAccessorToExpand.About);
                    _commandText.AppendFormat(" <{0}> ?{1}_type . ", Rdf.type, (_variableNameOverride.ContainsKey(_entityAccessorToExpand.About) ? _variableNameOverride[_entityAccessorToExpand.About] : _entityAccessorToExpand.About.Name));
                    _commandText.AppendLine();
                }

                _commandText.Append("FILTER ( EXISTS { ");
                _commandText.AppendLine();
                _commandText.Append(_indentation);
                _commandText.AppendFormat("?{0} ", _currentEntityAccessor.Peek().About.Name);
                VisitComponent(entityTypeConstrain.Predicate);
                _commandText.Append(" ");
                VisitComponent(entityTypeConstrain.Value);
                _commandText.Append(" . } ");
                _commandText.AppendLine();
                foreach (Literal inheritedType in entityTypeConstrain.InheritedTypes)
                {
                    _commandText.Append("|| EXISTS { ");
                    _commandText.AppendLine();
                    _commandText.Append(_indentation);
                    _commandText.AppendFormat("?{0} ", _currentEntityAccessor.Peek().About.Name);
                    VisitComponent(entityTypeConstrain.Predicate);
                    _commandText.Append(" ");
                    VisitComponent(inheritedType);
                    _commandText.Append(" . } ");
                    _commandText.AppendLine();
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
            _commandText.Append(_indentation);
            VisitComponent(unboundConstrain.Subject);
            _commandText.Append(" ");
            VisitComponent(unboundConstrain.Predicate);
            _commandText.Append(" ");
            VisitComponent(unboundConstrain.Value);
            _commandText.Append(" . ");
            _commandText.AppendLine();
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
            _commandText.Append(_indentation);
            _commandText.Append("FILTER (");
            VisitComponent(filter.Expression);
            _commandText.Append(") ");
            _commandText.AppendLine();
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

        /// <summary>Visit a strong entity accessor.</summary>
        /// <param name="entityAccessor">Strong entity accessor to be visited.</param>
        protected override void VisitStrongEntityAccessor(StrongEntityAccessor entityAccessor)
        {
            _currentStrongEntityAccessorVisitDelegate(entityAccessor);
        }

        /// <summary>Visit an optional patterns.</summary>
        /// <param name="optionalPattern">Optional patterns accessor to be visited.</param>
        protected override void VisitOptionalPattern(OptionalPattern optionalPattern)
        {
            _commandText.Append(_indentation);
            _commandText.AppendLine("OPTIONAL { ");
            _indentation += "\t";
            foreach (EntityConstrain pattern in optionalPattern.Patterns)
            {
                VisitComponent(pattern);
            }

            _indentation = _indentation.Substring(0, _indentation.Length - 1);
            _commandText.Append(_indentation);
            _commandText.AppendLine("} ");
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
                _commandText.Append(_indentation);
                _commandText.AppendFormat("OFFSET {0} ", _entityAccessorToExpand.OwnerQuery.Offset);
                _commandText.AppendLine();
            }

            if (_entityAccessorToExpand.OwnerQuery.Limit >= 0)
            {
                _commandText.Append(_indentation);
                _commandText.AppendFormat("LIMIT {0} ", _entityAccessorToExpand.OwnerQuery.Limit);
                _commandText.AppendLine();
            }
        }
        #endregion

        #region Private methods
        private void VisitStrongEntityAccessorInternal(StrongEntityAccessor entityAccessor)
        {
            int startIndex = _commandText.Length;
            _currentEntityAccessor.Push(entityAccessor);
            _commandText.Append(_indentation);
            _commandText.AppendFormat("GRAPH ?G{0} {{ ", entityAccessor.About.Name);
            _commandText.AppendLine();
            _indentation += "\t";
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
                ExpandEntityAccessor(entityAccessor, ref startIndex);
            }

            if (entityAccessor.UnboundGraphName == null)
            {
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
                _commandText.AppendLine("} ");
                _commandText.Append(_indentation);
                _commandText.AppendFormat("GRAPH <{0}> {{ ?G{1} <{2}> ?{1} . }} ", MetaGraphUri, entityAccessor.About.Name, Foaf.primaryTopic);
                _commandText.AppendLine();
            }
            else
            {
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
                _commandText.AppendLine("} ");
            }

            _currentEntityAccessor.Pop();
        }

        private void VisitStrongEntityAccessorInternalQuick(StrongEntityAccessor entityAccessor)
        {
            VisitComponent(entityAccessor.About);
        }

        private void ExpandEntityAccessor(StrongEntityAccessor entityAccessor, ref int startIndex)
        {
            int currentStartIndex = _commandText.Length;
            _entityAccessorToExpand.FindAllComponents<Identifier>().Select(item => _variableNameOverride[item] = item.Name + "_sub").ToList();
            bool indentationChanged = false;
            if (_indentation.Length > 0)
            {
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                indentationChanged = true;
            }

            _commandText.Append(_indentation);
            _commandText.AppendLine("{ ");
            _indentation += "\t";
            _commandText.Append(_indentation);
            _commandText.Append("SELECT DISTINCT ");
            VisitComponent(_entityAccessorToExpand.About);
            _commandText.AppendLine();
            _commandText.Append(_indentation);
            _commandText.AppendLine("WHERE { ");
            _indentation += "\t";
            _commandText.Append(_indentation);
            _commandText.AppendFormat("GRAPH ?G{0} {{ ", _variableNameOverride[_entityAccessorToExpand.About]);
            _commandText.AppendLine();
            _indentation += "\t";
            foreach (QueryElement element in entityAccessor.Elements.SkipWhile(item => item is UnboundConstrain))
            {
                VisitComponent(element);
            }

            if (_entityAccessorToExpand.UnboundGraphName == null)
            {
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
                _commandText.AppendLine("} ");
                _commandText.Append(_indentation);
                _commandText.AppendFormat("GRAPH <{0}> {{ ?G{1} <{2}> ?{1} . }} ", MetaGraphUri, _variableNameOverride[_entityAccessorToExpand.About], Foaf.primaryTopic);
                _commandText.AppendLine();
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
                _commandText.AppendLine("} ");
            }
            else
            {
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
                _commandText.AppendLine("} ");
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
                _commandText.AppendLine("} ");
            }

            VisitQueryResultModifiers(_entityAccessorToExpand.OwnerQuery.OrderBy, _entityAccessorToExpand.OwnerQuery.Offset, _entityAccessorToExpand.OwnerQuery.Limit);
            _indentation = _indentation.Substring(0, _indentation.Length - 1);
            _commandText.Append(_indentation);
            _commandText.AppendLine("} ");
            _commandText.Append(_indentation);
            _commandText.Append("FILTER (");
            VisitComponent(_entityAccessorToExpand.About);
            _variableNameOverride.Clear();
            _commandText.Append("=");
            VisitComponent(_entityAccessorToExpand.About);
            _commandText.AppendLine(") ");

            int currentLength = _commandText.Length - currentStartIndex;
            string expansionText = _commandText.ToString().Substring(currentStartIndex, currentLength);
            _commandText = _commandText.Remove(currentStartIndex, currentLength).Insert(startIndex, expansionText);
            _visitedComponents.Update(startIndex, currentLength);
            if (indentationChanged)
            {
                _indentation += "\t";
            }
        }

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

            string currentIndentation = _indentation;
            _indentation = System.String.Empty;
            if ((binaryOperator.LeftOperand is EntityConstrain) && (binaryOperator.RightOperand is EntityConstrain))
            {
                _commandText.AppendFormat("{0}{{ ", (operatorString == "||" ? "EXISTS " : System.String.Empty));
                VisitComponent(binaryOperator.LeftOperand);
                TrimWhiteSpaces('.');
                if (operatorString == "||")
                {
                    _commandText.Append(" } || EXISTS { ");
                }

                VisitComponent(binaryOperator.RightOperand);
                TrimWhiteSpaces('.');
                _commandText.Append("} ");
            }
            else
            {
                if (binaryOperator.LeftOperand is EntityConstrain)
                {
                    _commandText.Append("EXISTS { ");
                    VisitComponent(binaryOperator.LeftOperand);
                    TrimWhiteSpaces('.');
                    _commandText.AppendFormat(" }} {0} ", operatorString);
                    if (binaryOperator.RightOperand is BinaryOperator)
                    {
                        _commandText.Append("(");
                    }

                    VisitComponent(binaryOperator.RightOperand);
                    if (binaryOperator.RightOperand is BinaryOperator)
                    {
                        _commandText.Append(")");
                    }
                }
                else
                {
                    if (binaryOperator.LeftOperand is BinaryOperator)
                    {
                        _commandText.Append("(");
                    }

                    VisitComponent(binaryOperator.LeftOperand);
                    if (binaryOperator.LeftOperand is BinaryOperator)
                    {
                        _commandText.Append(")");
                    }

                    _commandText.AppendFormat(" {0} EXISTS {{ ", operatorString);
                    VisitComponent(binaryOperator.RightOperand);
                    TrimWhiteSpaces('.');
                    _commandText.Append(" } ");
                }
            }

            _indentation = currentIndentation;
        }

        private void VisitEntityIsNullCheck(BinaryOperator binaryOperator)
        {
            if (binaryOperator.Member == MethodNames.Equal)
            {
                EntityConstrain entityConstrain;
                _commandText.Append("NOT EXISTS { ");
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

                _commandText.Append(" } ");
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

        private void ProcessSelectable(bool isSubQuery, ISelectableQueryComponent expression, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            System.Reflection.MethodInfo processSelectable = typeof(GenericSparqlQueryVisitor).GetMethod("ProcessSelectable" + expression.GetType().Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (processSelectable != null)
            {
                processSelectable.Invoke(this, new object[] { expression, isSubQuery, createVariableName, mainEntityAccessor });
            }

            foreach (IExpression selectableExpression in expression.Expressions)
            {
                VisitComponent(selectableExpression);
                _commandText.Append(" ");
            }
        }

        private void ProcessSelectableIdentifierEntityAccessor(IdentifierEntityAccessor expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if (_currentStrongEntityAccessorVisitDelegate == VisitStrongEntityAccessorInternal)
            {
                _currentStrongEntityAccessorVisitDelegate = VisitStrongEntityAccessorInternalQuick;
            }

            if (!isSubQuery)
            {
                if ((_metaGraphVariableName == null) && (_entityVariableName == null))
                {
                    _metaGraphVariableName = String.Format("G{0}", expression.About.Name);
                    _entityVariableName = _ownerVariableName = expression.About.Name + "_Distinct";
                }

                _subjectVariableName = expression.About.Name;
                _predicateVariableName = expression.About.Name + "_Predicate";
                _objectVariableName = expression.About.Name + "_Object";

                _commandText.AppendFormat(
                    "DISTINCT(?{2}) AS ?{1} ?{0} ?{3} ?{4} ?{5} ",
                    _metaGraphVariableName,
                    _entityVariableName,
                    expression.About.Name,
                    _predicateVariableName,
                    _objectVariableName,
                    _ownerVariableName);
            }
        }

        private void ProcessSelectableStrongEntityAccessor(StrongEntityAccessor expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if (_currentStrongEntityAccessorVisitDelegate == VisitStrongEntityAccessorInternal)
            {
                _currentStrongEntityAccessorVisitDelegate = VisitStrongEntityAccessorInternalQuick;
            }

            if (!isSubQuery)
            {
                if ((expression.UnboundGraphName != null) && (expression.UnboundGraphName != expression.About))
                {
                    _metaGraphVariableName = (_metaGraphVariableName == null ? "graph" : _metaGraphVariableName);
                    _entityVariableName = (_entityVariableName == null ? expression.About.Name : _entityVariableName);
                    _ownerVariableName = (_ownerVariableName == null ? expression.About.Name : _ownerVariableName);
                    _commandText.AppendFormat("IF(BOUND(?G{0}),?G{0},?G{1}) AS ?{2} ", expression.About.Name, expression.UnboundGraphName.Name, _metaGraphVariableName);
                }
                else
                {
                    _entityVariableName = (_entityVariableName == null ? expression.About.Name : _entityVariableName);
                    _metaGraphVariableName = (_metaGraphVariableName == null ? String.Format("G{0}", _entityVariableName) : _metaGraphVariableName);
                    _ownerVariableName = (_ownerVariableName == null ? expression.About.Name : _ownerVariableName);
                    _commandText.AppendFormat("?{0} ?{1} ", _metaGraphVariableName, _entityVariableName);
                }
            }
        }

        private void ProcessSelectableUnboundConstrain(UnboundConstrain expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if ((!isSubQuery) && (expression.Subject is Identifier) && (expression.Predicate is Identifier) && (expression.Value is Identifier))
            {
                _subjectVariableName = ((Identifier)expression.Subject).Name;
                _predicateVariableName = ((Identifier)expression.Predicate).Name;
                _objectVariableName = ((Identifier)expression.Value).Name;
            }
        }

        private void ProcessSelectableCall(Call expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if ((!isSubQuery) && (_scalarVariableName == null))
            {
                _scalarVariableName = createVariableName(expression.Member.ToString());
            }
        }

        private void ProcessSelectableAlias(Alias expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if ((!isSubQuery) && (_scalarVariableName == null) && (expression.Name != null))
            {
                _scalarVariableName = expression.Name.Name;
            }
        }

        private void ProcessSelectableIdentifier(Identifier expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if (!isSubQuery)
            {
                if (_subjectVariableName == null)
                {
                    _subjectVariableName = expression.Name;
                }
                else if (_predicateVariableName == null)
                {
                    _predicateVariableName = expression.Name;
                }
                else if (_objectVariableName == null)
                {
                    _objectVariableName = expression.Name;
                }
                else if ((_entityVariableName == null) || (_entityVariableName == _ownerVariableName))
                {
                    _entityVariableName = expression.Name;
                }
            }
        }

        private void ProcessSelectableConditionalConstrainSelector(ConditionalConstrainSelector expression, bool isSubQuery, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if (!isSubQuery)
            {
                if (mainEntityAccessor != null)
                {
                    _currentStrongEntityAccessorVisitDelegate = entityAccessor =>
                        {
                            if (_metaGraphVariableName == null)
                            {
                                _commandText.AppendFormat("?G{0}", entityAccessor.About.Name);
                            }
                            else
                            {
                                _commandText.Append("IF(ISBLANK(");
                                VisitComponent(entityAccessor.About);
                                _commandText.Append("),");
                                VisitComponent(mainEntityAccessor.About);
                                _commandText.Append(",");
                                VisitComponent(entityAccessor.About);
                                _commandText.AppendFormat(") AS ?{0} ?{1}", _ownerVariableName, _entityVariableName);
                            }
                        };

                    _ownerVariableName = "entity";
                }

                _commandText.Append("DISTINCT ");
                int index = 0;
                foreach (IExpression selectable in ((ISelectableQueryComponent)expression).Expressions)
                {
                    switch (index)
                    {
                        case 0:
                            _subjectVariableName = (selectable is Alias ? ((Alias)selectable).Name.Name : (selectable is Identifier ? ((Identifier)selectable).Name : null));
                            break;
                        case 1:
                            _predicateVariableName = (selectable is Alias ? ((Alias)selectable).Name.Name : (selectable is Identifier ? ((Identifier)selectable).Name : null));
                            break;
                        case 2:
                            _objectVariableName = (selectable is Alias ? ((Alias)selectable).Name.Name : (selectable is Identifier ? ((Identifier)selectable).Name : null));
                            break;
                    }

                    index++;
                }
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

        private void BeginQuery(bool isSubQuery, QueryForms queryForm, IList<ISelectableQueryComponent> select, Func<string, string> createVariableName, StrongEntityAccessor mainEntityAccessor)
        {
            if ((_commandText.Length > 0) && (_commandText[_commandText.Length - 1] != '\n'))
            {
                _commandText.AppendLine();
            }

            if (isSubQuery)
            {
                _indentation += "\t";
            }

            _commandText.Append(_indentation);
            _commandText.AppendFormat("{0} ", queryForm.ToString().ToUpper());
            if (queryForm == QueryForms.Select)
            {
                if (select.Count > 0)
                {
                    Action<StrongEntityAccessor> temp = _currentStrongEntityAccessorVisitDelegate;
                    foreach (ISelectableQueryComponent expression in select)
                    {
                        ProcessSelectable(isSubQuery, expression, createVariableName, mainEntityAccessor);
                    }

                    _currentStrongEntityAccessorVisitDelegate = temp;
                }
                else
                {
                    _commandText.Append("* ");
                }
            }

            _commandText.AppendLine();
            if (queryForm != QueryForms.Ask)
            {
                _commandText.Append(_indentation);
                _commandText.Append("WHERE ");
            }

            _commandText.Append("{ ");
            _commandText.AppendLine();
            _indentation += "\t";
        }

        private void VisitOrderBy(IDictionary<IExpression, bool> orderByExpressions)
        {
            if (orderByExpressions.Any())
            {
                _commandText.Append(_indentation);
                _commandText.Append("ORDER BY ");
                foreach (KeyValuePair<IExpression, bool> orderBy in orderByExpressions)
                {
                    _commandText.Append(orderBy.Value ? "DESC(" : System.String.Empty);
                    VisitComponent(orderBy.Key);
                    _commandText.Append(orderBy.Value ? ") " : " ");
                }

                _commandText.AppendLine();
            }
        }

        private void EndQuery(bool isSubQuery, IDictionary<IExpression, bool> orderByExpressions)
        {
            _indentation = _indentation.Substring(0, _indentation.Length - 1);
            _commandText.Append(_indentation);
            _commandText.Append("} ");
            if (orderByExpressions != null)
            {
                VisitOrderBy(orderByExpressions);
            }

            if (isSubQuery)
            {
                _commandText.AppendLine("} ");
                _indentation = _indentation.Substring(0, _indentation.Length - 1);
                _commandText.Append(_indentation);
            }
        }

        private void VisitNullLiteral(Type literalType)
        {
            throw new NotSupportedException("Null literals are not supported in SPARQL.");
        }

        private void TrimWhiteSpaces(params char[] optionalCharactersToTrim)
        {
            int startIndex = _commandText.Length;
            for (int index = _commandText.Length - 1; index >= 0; index--)
            {
                if ((!Char.IsWhiteSpace(_commandText[index])) && ((optionalCharactersToTrim == null) ||
                    ((optionalCharactersToTrim != null) && (!optionalCharactersToTrim.Contains(_commandText[index])))))
                {
                    startIndex = index + 1;
                    break;
                }
            }

            if ((startIndex >= 0) && (startIndex < _commandText.Length - 1))
            {
                _commandText.Remove(startIndex, _commandText.Length - startIndex);
            }
        }
        #endregion
    }
}