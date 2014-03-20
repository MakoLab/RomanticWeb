using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    public interface IMappingProvider
    {
        void Accept(IMappingProviderVisitor visitor);
    }
}