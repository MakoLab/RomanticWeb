using System;
using System.Globalization;
using System.Text;
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
        #endregion

        #region Properties
        /// <summary>Gets a command text string.</summary>
        public string CommandText { get { return (_commandText!=null?_commandText.ToString():System.String.Empty); } }
        #endregion

        #region Public methods
        /// <summary>Visit a query model.</summary>
        /// <param name="queryModel">Query model to be visited.</param>
        public override void VisitQueryModel(QueryModel queryModel)
        {
            _metaGraphUri=queryModel.MetaGraphUri;
            base.VisitQueryModel(queryModel);
        }
        #endregion

        #region Non-public methods
        /// <summary>Visit a query.</summary>
        /// <param name="query">Query to be visited.</param>
        protected override void VisitQuery(Query query)
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

            _commandText.Append("SELECT ");
            if (query.Select.Count>0)
            {
                foreach (ISelectableQueryComponent expression in query.Select)
                {
                    if ((expression is EntityAccessor)&&(!query.IsSubQuery))
                    {
                        _commandText.AppendFormat("?G{0} ",((EntityAccessor)expression).About.Name);
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

            _commandText.Append("WHERE { ");
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

        /// <summary>Visit a function call.</summary>
        /// <param name="call">Function call to be visited.</param>
        protected override void VisitCall(Call call)
        {
            string functionName=call.Member.ToString().ToUpper();
            bool useBrackets=true;
            switch (call.Member)
            {
                case MethodNames.Any:
                    functionName="EXISTS";
                    useBrackets=false;
                    break;
            }

            _commandText.AppendFormat("{0}{1}",functionName,(useBrackets?"(":" "));
            int index=0;
            foreach (IExpression argument in call.Arguments)
            {
                VisitComponent(argument);
                if (index<call.Arguments.Count-1)
                {
                    _commandText.Append(",");
                }

                index++;
            }

            _commandText.Append((useBrackets?")":" "));
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
                    valueString=System.String.Format("<{0}>",literal.Value);
                    break;
            }

            _commandText.Append(valueString);
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