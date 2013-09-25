using System;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>
    /// Base class for mapping attributes
    /// </summary>
	public abstract class MappingAttribute:Attribute
	{
		#region Fields

	    private readonly string _prefix;
		#endregion

		#region Constructors

	    protected MappingAttribute(string prefix)
		{
			_prefix=prefix;
		}

        #endregion

		#region Properties
        /// <summary>
        /// Gets the ontology prefix
        /// </summary>
		public string Prefix { get { return _prefix; } }
		#endregion
	}
}