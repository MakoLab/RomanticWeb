using System;

namespace RomanticWeb.Mapping.Attributes
{
	public abstract class MappingAttribute:Attribute
	{
		#region Fields

	    private readonly string _prefix;
		#endregion

		#region Constructors

	    protected MappingAttribute(string prefix)
		{
			this._prefix=prefix;
		}

        #endregion

		#region Properties
		public string Prefix { get { return this._prefix; } }
		#endregion
	}
}