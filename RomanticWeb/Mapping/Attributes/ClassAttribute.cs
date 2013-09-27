using System;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>
    /// Maps a type to an RDF class
    /// </summary>
	[AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct)]
	public sealed class ClassAttribute:MappingAttribute
	{
		#region Fields
		private readonly string _className;
		#endregion

		#region Constructors
		public ClassAttribute(string prefix,string className):base(prefix)
		{
			_className=className;
		}
		#endregion

		#region Properties
		public string ClassName { get { return _className; } }
		#endregion

	    internal IClassMapping GetMapping(IOntologyProvider prefixes)
	    {
	        return new ClassMapping(prefixes.ResolveUri(Prefix,ClassName));
	    }
	}
}