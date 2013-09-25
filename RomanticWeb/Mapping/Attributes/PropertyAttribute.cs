using System;
using System.Linq;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>
    /// Maps a property to an RDF predicate
    /// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute:MappingAttribute
	{
		#region Fields
		private readonly string _propertyName;

		private Uri[] _range=new Uri[0];

		private int _cardinality=Int32.MaxValue;
		#endregion

		#region Constructors
		public PropertyAttribute(string prefix,string propertyName):base(prefix)
		{
			_propertyName=propertyName;
		}
		#endregion

		#region Properties
		public string PropertyName { get { return _propertyName; } }

		public Uri[] Range { get { return _range; } set { _range=value; } }

		public int Cardinality { get { return _cardinality; } set { _cardinality=value; } }
		#endregion

	    internal IPropertyMapping GetMapping(string propertyName,IOntologyProvider prefixes)
	    {
            return new PropertyMapping(propertyName,prefixes.ResolveUri(Prefix, PropertyName), null, false);
	    }
	}
}