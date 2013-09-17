using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.MetaData
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RdfObjectPropertyAttribute:RdfPropertyAttribute
	{
		#region Constructors
		public RdfObjectPropertyAttribute(string prefix,string baseUri,string propertyName):base(prefix,baseUri,propertyName)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,Uri baseUri,string propertyName):base(prefix,baseUri,propertyName)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,string baseUri,string propertyName,int cardinality):base(prefix,baseUri,propertyName,cardinality)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,Uri baseUri,string propertyName,int cardinality):base(prefix,baseUri,propertyName,cardinality)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,string baseUri,string propertyName,string[] range):base(prefix,baseUri,propertyName,range)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,Uri baseUri,string propertyName,Uri[] range):base(prefix,baseUri,propertyName,range)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,string baseUri,string propertyName,int cardinality,string[] range):base(prefix,baseUri,propertyName,cardinality,range)
		{
		}

		public RdfObjectPropertyAttribute(string prefix,Uri baseUri,string propertyName,int cardinality,Uri[] range):base(prefix,baseUri,propertyName,cardinality,range)
		{
		}
		#endregion
	}
}