using System;

namespace RomanticWeb.Mapping.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ObjectPropertyAttribute:PropertyAttribute
	{
		#region Constructors
		public ObjectPropertyAttribute(string prefix,string propertyName):base(prefix,propertyName)
		{
		}

		public ObjectPropertyAttribute(string prefix,string propertyName,int cardinality):base(prefix,propertyName,cardinality)
		{
		}

		public ObjectPropertyAttribute(string prefix,string propertyName,string[] range):base(prefix,propertyName,range)
		{
		}

		public ObjectPropertyAttribute(string prefix,string propertyName,int cardinality,string[] range):base(prefix,propertyName,cardinality,range)
		{
		}
		#endregion
	}
}