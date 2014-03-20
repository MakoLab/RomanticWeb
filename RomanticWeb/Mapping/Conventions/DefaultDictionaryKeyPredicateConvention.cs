using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    public class DefaultDictionaryKeyPredicateConvention : IDictionaryConvention
    {
        public bool ShouldApply(IDictionaryMappingProvider target)
        {
            return target.Key.GetTerm==null;
        }

        public void Apply(IDictionaryMappingProvider target)
        {
            target.Key.GetTerm=provider => Vocabularies.Rdf.predicate;
        }
    }
}