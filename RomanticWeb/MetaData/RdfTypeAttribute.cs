using System;

namespace RomanticWeb.MetaData
{
	[AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct)]
	public sealed class RdfTypeAttribute:OntologyMappingAttribute
	{
		#region Fields
		private string _className;
		#endregion

		#region Constructors
		public RdfTypeAttribute(string prefix,string baseUri,string className):base(prefix,baseUri)
		{
			_className=className;
		}

		public RdfTypeAttribute(string prefix,Uri baseUri,string className):base(prefix,baseUri)
		{
			_className=className;
		}
		#endregion

		#region Properties
		public string ClassName { get { return _className; } internal set { _className=value; } }
		#endregion
	}
}