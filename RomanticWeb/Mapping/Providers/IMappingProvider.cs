using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider
    /// </summary>
    public interface IMappingProvider
    {
        /// <summary>
        /// Accepts the specified mapping provider visitor.
        /// </summary>
        /// <param name="mappingProviderVisitor">The mapping provider visitor.</param>
        void Accept(IMappingProviderVisitor mappingProviderVisitor);
    }
}