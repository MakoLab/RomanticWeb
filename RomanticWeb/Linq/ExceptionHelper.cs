using System;
using System.Text.RegularExpressions;

namespace RomanticWeb.Linq
{
    /// <summary>Provides usefull common exception related methods.</summary>
    internal static class ExceptionHelper
    {
        #region Fields
        private static Regex PascalCaseSplitRegularExpression=new Regex("([A-Z]+[a-z]+)");
        #endregion

        #region Internal methods
        /// <summary>Throws an <see cref="System.ArgumentOutOfRangeException"/>.</summary>
        /// <param name="argumentName">Name of the generic type argument.</param>
        /// <param name="expectedType">Expected type.</param>
        /// <param name="foundType">Passed type.</param>
        internal static void ThrowGenericArgumentOutOfRangeException(string argumentName,Type expectedType,Type foundType)
        {
            throw new ArgumentOutOfRangeException(argumentName,String.Format("Expected '{0}' derived type, but found '{1}'.",expectedType.FullName,foundType.FullName));
        }

        /// <summary>Throws an <see cref="System.InvalidCastException"/>.</summary>
        /// <param name="expectedType">Expected type.</param>
        /// <param name="foundType">Passed type.</param>
        internal static void ThrowInvalidCastException(Type expectedType,Type foundType)
        {
            throw new InvalidCastException(String.Format("Expected '{0}' type, found '{1}'.",expectedType.FullName,foundType.FullName));
        }

        /// <summary>Throws a <see cref="System.NotSupportedException"/>.</summary>
        /// <param name="expressionText">Unsuported expression text.</param>
        internal static void ThrowNotSupportedException(string expressionText)
        {
            throw new NotSupportedException(String.Format("Expression of type '{0}' is not supported.",expressionText));
        }

        /// <summary>Throws a <see cref="Mapping.MappingException" />.</summary>
        /// <param name="predicate">Predicate Uri for which mapping was not found.</param>
        internal static void ThrowMappingException(Uri predicate)
        {
            throw new Mapping.MappingException(System.String.Format("Mapping for predicate '{0}' was not found.",predicate));
        }
        #endregion
    }
}