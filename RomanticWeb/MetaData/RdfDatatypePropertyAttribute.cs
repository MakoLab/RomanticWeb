using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.MetaData
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RdfDatatypePropertyAttribute:RdfPropertyAttribute
	{
		#region Constructors
		public RdfDatatypePropertyAttribute(string prefix,string baseUri,string propertyName):base(prefix,baseUri,propertyName)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,Uri baseUri,string propertyName):base(prefix,baseUri,propertyName)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,string baseUri,string propertyName,int cardinality):base(prefix,baseUri,propertyName,cardinality)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,Uri baseUri,string propertyName,int cardinality):base(prefix,baseUri,propertyName,cardinality)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,string baseUri,string propertyName,string[] range):base(prefix,baseUri,propertyName,range)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,Uri baseUri,string propertyName,Uri[] range):base(prefix,baseUri,propertyName,range)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,string baseUri,string propertyName,int cardinality,string[] range):base(prefix,baseUri,propertyName,cardinality,range)
		{
		}

		public RdfDatatypePropertyAttribute(string prefix,Uri baseUri,string propertyName,int cardinality,Uri[] range):base(prefix,baseUri,propertyName,cardinality,range)
		{
		}
		#endregion
	}
}