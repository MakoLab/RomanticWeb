using System;
using System.Linq;
using System.Reflection;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Defines a navigator type for given query component.</summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    internal class QueryComponentNavigatorAttribute:Attribute
    {
        #region Fields
        private Type _navigatorType;
        private ConstructorInfo _constructor;
        #endregion

        #region Constructors
        /// <summary>Constructor with navigator type passed.</summary>
        /// <param name="navigatorType">Navigator type.</param>
        internal QueryComponentNavigatorAttribute(Type navigatorType)
        {
            if (!(typeof(IQueryComponentNavigator)).IsAssignableFrom(navigatorType))
            {
                throw new ArgumentOutOfRangeException("navigatorType");
            }

            _constructor=navigatorType.GetConstructors(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)
                .Where(item => (item.GetParameters().Length==1)&&(typeof(IQueryComponent).IsAssignableFrom(item.GetParameters()[0].ParameterType)))
                .FirstOrDefault();

            if (_constructor==null)
            {
                throw new ArgumentOutOfRangeException("navigatorType");
            }

            _navigatorType=navigatorType;
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigator type.</summary>
        public Type NavigatorType { get { return _navigatorType; } }

        internal ConstructorInfo Constructor { get { return _constructor; } }
        #endregion
    }
}