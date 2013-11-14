using System;
using System.Reflection;
using RomanticWeb.Linq.Model;

namespace RomanticWeb.Linq.Visitor
{
    /// <summary>Provides an abstraction layer on entity query model.</summary>
    public abstract class QueryVisitorBase
    {
        #region Public methods
        /// <summary>Visit a query component.</summary>
        /// <param name="component">Component to be visited.</param>
        public void VisitComponent(IQueryComponent component)
        {
            Type componentType=GetType();
            MethodInfo componentMethodInfo=null;
            while ((componentType!=typeof(object))&&(componentMethodInfo==null))
            {
                componentMethodInfo=componentType.GetMethod("Visit"+component.GetType().Name,BindingFlags.NonPublic|BindingFlags.Instance);
                componentType=componentType.BaseType;
            }

            if (componentMethodInfo!=null)
            {
                componentMethodInfo.Invoke(this,new object[] { component });
            }
        }

        /// <summary>Visit a query model.</summary>
        /// <param name="queryModel">Query model to be visited.</param>
        public virtual void VisitQueryModel(QueryModel queryModel)
        {
            VisitQuery(queryModel.Query);
        }
        #endregion

        #region Non-public methods
        /// <summary>Visit a query.</summary>
        /// <param name="query">Query to be visited.</param>
        protected abstract void VisitQuery(Query query);

        /// <summary>Visit a function call.</summary>
        /// <param name="call">Function call to be visited.</param>
        protected abstract void VisitCall(Call call);

        /// <summary>Visit an unary operator.</summary>
        /// <param name="unaryOperator">Unary operator to be visited.</param>
        protected abstract void VisitUnaryOperator(UnaryOperator unaryOperator);

        /// <summary>Visit a binary operator.</summary>
        /// <param name="binaryOperator">Binary operator to be visited.</param>
        protected abstract void VisitBinaryOperator(BinaryOperator binaryOperator);

        /// <summary>Visit an entity constrain.</summary>
        /// <param name="entityConstrain">Entity constrain to be visited.</param>
        protected abstract void VisitEntityConstrain(EntityConstrain entityConstrain);

        /// <summary>Visit an unbound constrain.</summary>
        /// <param name="unboundConstrain">Unbound constrain to be visited.</param>
        protected abstract void VisitUnboundConstrain(UnboundConstrain unboundConstrain);

        /// <summary>Visit a literal.</summary>
        /// <param name="literal">Literal to be visited.</param>
        protected abstract void VisitLiteral(Literal literal);

        /// <summary>Visit a prefix.</summary>
        /// <param name="prefix">Prefix to be visited.</param>
        protected abstract void VisitPrefix(Prefix prefix);

        /// <summary>Visit a identifier.</summary>
        /// <param name="identifier">Identifier to be visited.</param>
        protected abstract void VisitIdentifier(Identifier identifier);

        /// <summary>Visit a filter.</summary>
        /// <param name="filter">Filter to be visited.</param>
        protected abstract void VisitFilter(Filter filter);

        /// <summary>Visit an entity accessor.</summary>
        /// <param name="entityAccessor">Entity accessor to be visited.</param>
        protected abstract void VisitEntityAccessor(EntityAccessor entityAccessor);
        #endregion
    }
}