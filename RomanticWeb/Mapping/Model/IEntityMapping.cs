namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// A higeset level of entity mapping,
    /// used to access property mappings, type mappings, etc.
    /// </summary>
	public interface IEntityMapping
	{
        /// <summary>
        /// Gets the RDF type mapping
        /// </summary>
		ITypeMapping Type { get; }

        /// <summary>
        /// Gets the property mapping for a property by name
        /// </summary>
		IPropertyMapping PropertyFor(string propertyName);
	}
}