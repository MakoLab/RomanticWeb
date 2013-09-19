using System;
using System.Linq;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute:MappingAttribute
	{
		#region Fields
		private string _propertyName;

		private Uri[] _range=new Uri[0];

		private int _cardinality=Int32.MaxValue;
		#endregion

		#region Constructors
		public PropertyAttribute(string prefix,string propertyName):base(prefix)
		{
			this._propertyName=propertyName;
		}

		public PropertyAttribute(string prefix,string propertyName,int cardinality):this(prefix,propertyName)
		{
			if (cardinality>0)
			{
				this._cardinality=cardinality;
			}
		}

		public PropertyAttribute(string prefix,string propertyName,string[] range):this(prefix,propertyName)
		{
			if (range!=null)
			{
				this._range=range.Select(item => new Uri(item)).ToArray();
			}
		}

		public PropertyAttribute(string prefix,string propertyName,int cardinality,string[] range):this(prefix,propertyName,range)
		{
			if (cardinality>0)
			{
				this._cardinality=cardinality;
			}
		}
		#endregion

		#region Properties
		public string PropertyName { get { return this._propertyName; } internal set { this._propertyName=value; } }

		public Uri[] Range { get { return this._range; } internal set { this._range=value; } }

		public int Cardinality { get { return this._cardinality; } internal set { this._cardinality=value; } }
		#endregion

	    internal IPropertyMapping GetMapping(string propertyName,IOntologyProvider prefixes)
	    {
            return new PropertyMapping(propertyName,prefixes.ResolveUri(this.Prefix, this.PropertyName), null, false);
	    }
	}
}