using System;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Attributes
{
	[AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct)]
	public sealed class RdfTypeAttribute:MappingAttribute
	{
		#region Fields
		private string _className;
		#endregion

		#region Constructors
		public RdfTypeAttribute(string prefix,string className):base(prefix)
		{
			this._className=className;
		}
		#endregion

		#region Properties
		public string ClassName { get { return this._className; } internal set { this._className=value; } }
		#endregion

	    internal ITypeMapping GetMapping(IOntologyProvider prefixes)
	    {
	        return new TypeMapping(prefixes.ResolveUri(this.Prefix,this.ClassName));
	    }
	}
}