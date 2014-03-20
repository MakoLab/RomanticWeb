using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    public class DefaultDictionaryValuePredicateConvention:IDictionaryConvention
    {
        public bool ShouldApply(IDictionaryMappingProvider target)
        {
            return target.Value.GetTerm == null;
        }

        public void Apply(IDictionaryMappingProvider target)
        {
            target.Value.GetTerm=provider => Vocabularies.Rdf.@object;
        }
    }
}