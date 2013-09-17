using System;

namespace RomanticWeb.MetaData
{
	public abstract class OntologyMappingAttribute:Attribute
	{
		#region Fields
		private Uri _baseUri;
		private string _prefix;
		#endregion

		#region Constructors
		public OntologyMappingAttribute(string prefix,string baseUri)
		{
			_prefix=prefix;
			_baseUri=new Uri(baseUri);
		}

		public OntologyMappingAttribute(string prefix,Uri baseUri)
		{
			_prefix=prefix;
			_baseUri=baseUri;
		}
		#endregion

		#region Properties
		public Uri BaseUri { get { return _baseUri; } internal set { _baseUri=value; } }

		public string Prefix { get { return _prefix; } internal set { _prefix=value; } }
		#endregion
	}
}