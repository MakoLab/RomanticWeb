using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Conventions
{
    public class ConventionsVisitor:IMappingProviderVisitor
    {
        private readonly IList<IConvention> _conventions;

        public ConventionsVisitor(IEnumerable<IConvention> conventions)
        {
            _conventions=conventions.ToList();
        }

        public void Visit(ICollectionMappingProvider mappingProvider)
        {
            Visit(mappingProvider as IPropertyMappingProvider);
            SelectAndApplyConventions<ICollectionConvention,ICollectionMappingProvider>(mappingProvider);
        }

        public void Visit(IPropertyMappingProvider mappingProvider)
        {
            SelectAndApplyConventions<IPropertyConvention,IPropertyMappingProvider>(mappingProvider);
        }

        public void Visit(IDictionaryMappingProvider mappingProvider)
        {
            Visit(mappingProvider as IPropertyMappingProvider);
            SelectAndApplyConventions<IDictionaryConvention,IDictionaryMappingProvider>(mappingProvider);
        }

        public void Visit(IClassMappingProvider mappingProvider)
        {
        }

        public void Visit(IEntityMappingProvider mappingProvider)
        {
        }

        private void SelectAndApplyConventions<TConvention,TProvider>(TProvider provider)
            where TConvention:IConvention<TProvider>
        {
            var matchingConventions=from convention in _conventions.OfType<TConvention>()
                                    where convention.ShouldApply(provider)
                                    select convention;
            foreach (var convention in matchingConventions)
            {
                convention.Apply(provider);
            }
        }
    }
}