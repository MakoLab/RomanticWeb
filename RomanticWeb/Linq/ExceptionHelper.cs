using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;

namespace RomanticWeb.Linq
{
	internal static class ExceptionHelper
	{
		private static Regex PascalCaseSplitRegularExpression=new Regex("([A-Z]+[a-z]+)");

		internal static void ThrowGenericArgumentOutOfRangeException(string argumentName,Type expectedType,Type foundType)
		{
			throw new ArgumentOutOfRangeException(argumentName,System.String.Format("Expected '{0}' derived type, but found '{1}'.",expectedType.FullName,foundType.FullName));
		}

		internal static void ThrowInvalidCastException(Type expectedType,Type foundType)
		{
			throw new InvalidCastException(System.String.Format("Expected '{0}' type, found '{1}'.",expectedType.FullName,foundType.FullName));
		}

		internal static void ThrowNotSupportedException(string expressionText)
		{
			new NotSupportedException(System.String.Format("Expression of type '{0}' is not supported.",expressionText));
		}
	}
}