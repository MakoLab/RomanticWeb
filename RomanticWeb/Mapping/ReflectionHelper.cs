using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RomanticWeb.Mapping
{
	internal static class ReflectionHelper
	{
		public static PropertyInfo ExtractPropertyInfo(this LambdaExpression expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression==null)
			{
			    throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property", expression));
			}

			if (memberExpression.Member is FieldInfo)
			{
			    throw new NotImplementedException("Currently mapping fields is not supported");
			}

			return (PropertyInfo)memberExpression.Member;
		} 
	}
}