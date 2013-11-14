using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides details about filter.</summary>
    [QueryComponentNavigator(typeof(FilterNavigator))]
    public class Filter:QueryElement
    {
        #region Fields
        private IExpression _expression;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public Filter():base()
        {
        }

        /// <summary>Default constructor with filter expresion passed.</summary>
        /// <param name="expression">Filter expression.</param>
        public Filter(IExpression expression):base()
        {
            Expression=expression;
        }
        #endregion

        #region Properties
        /// <summary>Gets a filter expression.</summary>
        [AllowNull]
        public IExpression Expression { get { return _expression; } set { _expression=value; } }

        /// <summary>Gets an owning query.</summary>
        internal virtual Query OwnerQuery
        {
            get
            {
                return base.OwnerQuery;
            }

            set
            {
                base.OwnerQuery=value;
                if ((_expression!=null)&&(_expression is QueryComponent))
                {
                    ((QueryComponent)_expression).OwnerQuery=OwnerQuery;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this filter.</summary>
        /// <returns>String representation of this filter.</returns>
        public override string ToString()
        {
            return System.String.Format("FILTER ({0})",(_expression!=null?_expression.ToString():System.String.Empty));
        }
        #endregion
    }
}