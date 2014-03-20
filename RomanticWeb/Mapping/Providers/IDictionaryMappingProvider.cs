namespace RomanticWeb.Mapping.Providers
{
    public interface IDictionaryMappingProvider:IPropertyMappingProvider
    {
        ITermMappingProvider Key { get; }

        ITermMappingProvider Value { get; }
    }
}