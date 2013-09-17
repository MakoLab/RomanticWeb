using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

		internal static void ThrowInvalidOperationException(Expression expression)
		{
			string expressionType=expression.GetType().Name.Replace("Expression",System.String.Empty);
			expressionType=PascalCaseSplitRegularExpression.Replace(expressionType,match => (match.Value.Length>3?match.Value:match.Value.ToLower())+" ");
			throw new InvalidOperationException(System.String.Format("Unsupported {0} expression",expressionType));
		}
	}
}