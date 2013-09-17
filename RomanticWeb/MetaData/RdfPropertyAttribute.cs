using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.MetaData
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RdfPropertyAttribute:OntologyMappingAttribute
	{
		#region Fields
		private string _propertyName;

		private Uri[] _range=new Uri[0];

		private int _cardinality=Int32.MaxValue;
		#endregion

		#region Constructors
		public RdfPropertyAttribute(string prefix,string baseUri,string propertyName):base(prefix,baseUri)
		{
			_propertyName=propertyName;
		}

		public RdfPropertyAttribute(string prefix,Uri baseUri,string propertyName):base(prefix,baseUri)
		{
			_propertyName=propertyName;
		}

		public RdfPropertyAttribute(string prefix,string baseUri,string propertyName,int cardinality):this(prefix,baseUri,propertyName)
		{
			if (cardinality>0)
			{
				_cardinality=cardinality;
			}
		}

		public RdfPropertyAttribute(string prefix,Uri baseUri,string propertyName,int cardinality):this(prefix,baseUri,propertyName)
		{
			if (cardinality>0)
			{
				_cardinality=cardinality;
			}
		}

		public RdfPropertyAttribute(string prefix,string baseUri,string propertyName,string[] range):this(prefix,baseUri,propertyName)
		{
			if (range!=null)
			{
				_range=range.Select(item => new Uri(item)).ToArray();
			}
		}

		public RdfPropertyAttribute(string prefix,Uri baseUri,string propertyName,Uri[] range):this(prefix,baseUri,propertyName)
		{
			if (range!=null)
			{
				_range=range;
			}
		}

		public RdfPropertyAttribute(string prefix,string baseUri,string propertyName,int cardinality,string[] range):this(prefix,baseUri,propertyName,range)
		{
			if (cardinality>0)
			{
				_cardinality=cardinality;
			}
		}

		public RdfPropertyAttribute(string prefix,Uri baseUri,string propertyName,int cardinality,Uri[] range):this(prefix,baseUri,propertyName,range)
		{
			if (cardinality>0)
			{
				_cardinality=cardinality;
			}
		}
		#endregion

		#region Properties
		public string PropertyName { get { return _propertyName; } internal set { _propertyName=value; } }

		public Uri[] Range { get { return _range; } internal set { _range=value; } }

		public int Cardinality { get { return _cardinality; } internal set { _cardinality=value; } }
		#endregion
	}
}