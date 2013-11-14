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
            throw new ArgumentOutOfRangeException(argumentName,System.String.Format("Expected '{0}' derived type, but found '{1}'.",expectedType.FullName,foundType.FullName));
        }

        /// <summary>Throws an <see cref="System.InvalidCastException"/>.</summary>
        /// <param name="expectedType">Expected type.</param>
        /// <param name="foundType">Passed type.</param>
        internal static void ThrowInvalidCastException(Type expectedType,Type foundType)
        {
            throw new InvalidCastException(System.String.Format("Expected '{0}' type, found '{1}'.",expectedType.FullName,foundType.FullName));
        }

        /// <summary>Throws a <see cref="System.NotSupportedException"/>.</summary>
        /// <param name="expressionText">Unsuported expression text.</param>
        internal static void ThrowNotSupportedException(string expressionText)
        {
            new NotSupportedException(System.String.Format("Expression of type '{0}' is not supported.",expressionText));
        }
        #endregion
    }
}