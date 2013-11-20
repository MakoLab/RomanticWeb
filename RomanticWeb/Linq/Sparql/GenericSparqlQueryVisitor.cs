using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NullGuard;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Visitor;

namespace RomanticWeb.Linq.Sparql
{
    /// <summary>Povides a SPARQL query parsing mechanism.</summary>
    public class GenericSparqlQueryVisitor:QueryVisitorBase
    {
        #region Fields
        private StringBuilder _commandText=null;
        private Uri _metaGraphUri=null;
        private string _metaGraphVariableName=null;
        private string _entityVariableName=null;
        private string _subjectVariableName=null;
        private string _predicateVariableName=null;
        private string _objectVariableName=null;
        private string _scalarVariableName=null;
        #endregion

        #region Properties
        /// <summary>Gets a command text string.</summary>
        public string CommandText { get { return (_commandText!=null?_commandText.ToString():System.String.Empty); } }

        /// <summary>Sets a meta-graph URI.</summary>
        public override Uri MetaGraphUri { [return: AllowNull] get { return _metaGraphUri; } set { _metaGraphUri=value; } }

        /// <summary>Gets or sets a meta-graph variable name.</summary>
        public override string MetaGraphVariableName { [return: AllowNull] get { return _metaGraphVariableName; } set { _metaGraphVariableName=value; } }

        /// <summary>Gets or sets an entity variable name.</summary>
        public override string EntityVariableName { [return: AllowNull] get { return _entityVariableName; } set { _entityVariableName=value; } }

        /// <summary>Gets or sets a subject variable name.</summary>
        public override string SubjectVariableName { [return: AllowNull] get { return _subjectVariableName; } set { _subjectVariableName=value; } }

        /// <summary>Gets or sets a predicate variable name.</summary>
        public override string PredicateVariableName { [return: AllowNull] get { return _predicateVariableName; } set { _predicateVariableName=value; } }

        /// <summary>Gets or sets a object variable name.</summary>
        public override string ObjectVariableName { [return: AllowNull] get { return _objectVariableName; } set { _objectVariableName=value; } }

        /// <summary>Gets or sets a scalar variable name.</summary>
        public override string ScalarVariableName { [return: AllowNull] get { return _scalarVariableName; } set { _scalarVariableName=value; } }
        #endregion

        #region Public methods
        /// <summary>Visit a query.</summary>
        /// <param name="query">Query to be visited.</param>
        public override void VisitQuery(Query query)
        {
            if (_commandText==null)
            {
                _commandText=new StringBuilder();
            }

            if (query.IsSubQuery)
            {
                _commandText.Append("{ ");
            }
            else
            {
                VisitComponent(new Prefix("xsd",new Uri(RomanticWeb.Vocabularies.Xsd.BaseUri)));

                foreach (Prefix prefix in query.Prefixes)
                {
                    VisitComponent(prefix);
                }
            }

            _commandText.AppendFormat("{0} ",query.QueryForm.ToString().ToUpper());
            if (query.QueryForm==QueryForms.Select)
            {
                if (query.Select.Count>0)
                {
                    foreach (ISelectableQueryComponent expression in query.Select)
                    {
                        if (!query.IsSubQuery)
                        {
                            if (expression is EntityAccessor)
                            {
                                if ((_metaGraphVariableName==null)&&(_entityVariableName==null))
                                {
                                    _metaGraphVariableName=System.String.Format("G{0}",((EntityAccessor)expression).About.Name);
                                    _entityVariableName=((EntityAccessor)expression).About.Name;
                                }

                                _commandText.AppendFormat("?G{0} ",((EntityAccessor)expression).About.Name);
                            }
                            else if (expression is UnboundConstrain)
                            {
                                UnboundConstrain unboundConstrain=(UnboundConstrain)expression;
                                if ((unboundConstrain.Subject is Identifier)&&(unboundConstrain.Predicate is Identifier)&&(unboundConstrain.Value is Identifier))
                                {
                                    _subjectVariableName=((Identifier)unboundConstrain.Subject).Name;
                                    _predicateVariableName=((Identifier)unboundConstrain.Predicate).Name;
                                    _objectVariableName=((Identifier)unboundConstrain.Value).Name;
                                }
                            }
                            else if (expression is Call)
                            {
                                if (_scalarVariableName==null)
                                {
                                    _scalarVariableName=query.CreateVariableName(((Call)expression).Member.ToString().CamelCase());
                                }
                            }
                            else if (expression is Alias)
                            {
                                Alias alias=(Alias)expression;
                                if ((_scalarVariableName==null)&&(alias.Name!=null))
                                {
                                    _scalarVariableName=alias.Name.Name;
                                }
                            }
                        }

                        foreach (IExpression selectableExpression in expression.Expressions)
                        {
                            VisitComponent(selectableExpression);
                            _commandText.Append(" ");
                        }
                    }
                }
                else
                {
                    _commandText.Append("* ");
                }
            }

            if (query.QueryForm!=QueryForms.Ask)
            {
                _commandText.Append("WHERE ");
            }

            _commandText.Append("{ ");

            foreach (QueryElement element in query.Elements)
            {
                VisitComponent(element);
            }

            _commandText.Append("} ");

            if (query.IsSubQuery)
            {
                _commandText.Append("} ");
            }
        }
        #endregion

        #region Non-public methods
        /// <summary>Visit a function call.</summary>
        /// <param name="call">Function call to be visited.</param>
        protected override void VisitCall(Call call)
        {
            string functionName=call.Member.ToString().ToUpper();
            string openingBracket="(";
            string closingBracket=")";
            string separator=System.String.Empty;
            string targetAccessor=System.String.Empty;
            ICollection<IExpression> arguments=call.Arguments;
            IExpression target=null;
            switch (call.Member)
            {
                case MethodNames.Any:
                    functionName="EXISTS";
                    openingBracket=closingBracket=" ";
                    break;
                case MethodNames.In:
                    targetAccessor=separator=" ";
                    target=(call.Arguments.Count>0?call.Arguments.First():null);
                    arguments=(call.Arguments.Count>1?call.Arguments.Skip(1).ToList():new List<IExpression>());
                    break;
            }

            if (target!=null)
            {
                VisitComponent(target);
            }

            _commandText.AppendFormat("{0}{1}{2}{3}",targetAccessor,functionName,separator,openingBracket);
            int index=0;
            foreach (IExpression argument in arguments)
            {
                VisitComponent(argument);
                if (index<arguments.Count-1)
                {
                    _commandText.Append(",");
                }

                index++;
            }

            _commandText.AppendFormat("{0}",closingBracket);
        }

        /// <summary>Visit an unary operator.</summary>
        /// <param name="unaryOperator">Unary operator to be visited.</param>
        protected override void VisitUnaryOperator(UnaryOperator unaryOperator)
        {
            string operatorString=System.String.Empty;
            switch (unaryOperator.Member)
            {
                case MethodNames.Not:
                    operatorString="!";
                    break;
                default:
                    throw new NotImplementedException(System.String.Format("Unary operator '{0}' is not supported.",unaryOperator.Member));
            }

            _commandText.Append(operatorString);
            VisitComponent(unaryOperator.Operand);
        }

        /// <summary>Visit a binary operator.</summary>
        /// <param name="binaryOperator">Binary operator to be visited.</param>
        protected override void VisitBinaryOperator(BinaryOperator binaryOperator)
        {
            string operatorString=System.String.Empty;
            switch (binaryOperator.Member)
            {
                case MethodNames.Add:
                    operatorString="+";
                    break;
                case MethodNames.Substract:
                    operatorString="-";
                    break;
                case MethodNames.Multiply:
                    operatorString="*";
                    break;
                case MethodNames.Divide:
                    operatorString="/";
                    break;
                case MethodNames.Equal:
                    operatorString="=";
                    break;
                case MethodNames.GreaterThan:
                    operatorString=">";
                    break;
                case MethodNames.GreaterThanOrEqual:
                    operatorString=">=";
                    break;
                case MethodNames.LessThan:
                    operatorString="<";
                    break;
                case MethodNames.LessThanOrEqual:
                    operatorString="<=";
                    break;
                case MethodNames.NotEqual:
                    operatorString="!=";
                    break;
                case MethodNames.Or:
                    operatorString="||";
                    break;
                case MethodNames.And:
                    operatorString="&&";
                    break;
                default:
                    throw new NotImplementedException(System.String.Format("Binary operator '{0}' is not supported.",binaryOperator.Member));
            }

            VisitComponent(binaryOperator.LeftOperand);
            _commandText.Append(operatorString);
            VisitComponent(binaryOperator.RightOperand);
        }

        /// <summary>Visit an entity constrain.</summary>
        /// <param name="entityConstrain">Entity constrain to be visited.</param>
        protected override void VisitEntityConstrain(EntityConstrain entityConstrain)
        {
            VisitComponent(entityConstrain.Predicate);
            _commandText.Append(" ");
            VisitComponent(entityConstrain.Value);
            _commandText.Append(" . ");
        }

        /// <summary>Visit an unbound constrain.</summary>
        /// <param name="unboundConstrain">Unbound constrain to be visited.</param>
        protected override void VisitUnboundConstrain(UnboundConstrain unboundConstrain)
        {
            VisitComponent(unboundConstrain.Subject);
            _commandText.Append(" ");
            VisitEntityConstrain((EntityConstrain)unboundConstrain);
        }

        /// <summary>Visit a literal.</summary>
        /// <param name="literal">Literal to be visited.</param>
        protected override void VisitLiteral(Literal literal)
        {
            string valueString=System.String.Empty;
            switch (literal.Value.GetType().FullName)
            {
                default:
                    valueString=System.String.Format("\"{0}\"",literal.Value);
                    break;
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                    valueString=literal.Value.ToString();
                    break;
                case "System.Char":
                case "System.String":
                    valueString=System.String.Format("\"{0}\"^^xsd:string",literal.Value);
                    break;
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    valueString=System.String.Format(CultureInfo.InvariantCulture,"{0}",literal.Value);
                    break;
                case "System.DateTime":
                    valueString=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:dateTime",literal.Value);
                    break;
                case "System.Uri":
                case "RomanticWeb.Entities.EntityId":
                    valueString=System.String.Format("<{0}>",literal.Value);
                    break;
            }

            _commandText.Append(valueString);
        }

        /// <summary>Visit a list.</summary>
        /// <param name="list">List to be visited.</param>
        protected override void VisitList(List list)
        {
            int index=0;
            foreach (Literal literal in list.Values)
            {
                VisitComponent(literal);
                if (index<list.Values.Count-1)
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
            _commandText.AppendFormat("PREFIX {0}: <{1}> ",prefix.NamespacePrefix,prefix.NamespaceUri);
        }

        /// <summary>Visit a identifier.</summary>
        /// <param name="identifier">Identifier to be visited.</param>
        protected override void VisitIdentifier(Identifier identifier)
        {
            _commandText.AppendFormat("?{0}",identifier.Name);
        }

        /// <summary>Visit a filter.</summary>
        /// <param name="filter">Filter to be visited.</param>
        protected override void VisitFilter(Filter filter)
        {
            _commandText.Append("FILTER (");
            VisitComponent(filter.Expression);
            _commandText.Append(") ");
        }

        /// <summary>Visit an entity accessor.</summary>
        /// <param name="entityAccessor">Entity accessor to be visited.</param>
        protected override void VisitEntityAccessor(EntityAccessor entityAccessor)
        {
            _commandText.AppendFormat("GRAPH ?G{0} {{ ",entityAccessor.About.Name);
            foreach (QueryElement element in entityAccessor.Elements)
            {
                if (element.GetType()==typeof(EntityConstrain))
                {
                    _commandText.AppendFormat("?{0} ",entityAccessor.About.Name);
                }

                VisitComponent(element);
            }

            _commandText.Append("} ");
            _commandText.AppendFormat("GRAPH <{0}> {{ ?G{1} <http://xmlns.com/foaf/0.1/primaryTopic> ",_metaGraphUri,entityAccessor.About.Name);
            VisitComponent(entityAccessor.About);
            _commandText.Append(" . } ");
        }
        #endregion
    }
}