using System;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>
    /// Maps a type to an RDF class
    /// </summary>
	[AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct)]
	public sealed class ClassTypeAttribute:MappingAttribute
	{
		#region Fields
		private readonly string _className;
		#endregion

		#region Constructors
		public ClassTypeAttribute(string prefix,string className):base(prefix)
		{
			_className=className;
		}
		#endregion

		#region Properties
		public string ClassName { get { return _className; } }
		#endregion

	    internal ITypeMapping GetMapping(IOntologyProvider prefixes)
	    {
	        return new TypeMapping(prefixes.ResolveUri(Prefix,ClassName));
	    }
	}
}