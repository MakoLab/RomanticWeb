using System;

namespace RomanticWeb.Mapping.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DatatypePropertyAttribute:PropertyAttribute
	{
		#region Constructors
		public DatatypePropertyAttribute(string prefix,string propertyName):base(prefix,propertyName)
		{
		}

		public DatatypePropertyAttribute(string prefix,string propertyName,int cardinality):base(prefix,propertyName,cardinality)
		{
		}

		public DatatypePropertyAttribute(string prefix,string propertyName,string[] range):base(prefix,propertyName,range)
		{
		}

		public DatatypePropertyAttribute(string prefix,string propertyName,int cardinality,string[] range):base(prefix,propertyName,cardinality,range)
		{
		}

		#endregion
	}
}