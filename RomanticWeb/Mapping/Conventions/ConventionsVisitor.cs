using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// Visits mapping providers and applies conventions
    /// </summary>
    public class ConventionsVisitor:IMappingProviderVisitor
    {
        private readonly IList<IConvention> _conventions;

        /// <summary>Initializes a new instance of the <see cref="ConventionsVisitor"/> class.</summary>
        public ConventionsVisitor(MappingContext mappingContext)
        {
            _conventions=mappingContext.Conventions.ToList();
        }

        /// <summary>Applies property and collection conventions to <paramref name="collectionMappingProvider"/>.</summary>
        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
            Visit(collectionMappingProvider as IPropertyMappingProvider);
            SelectAndApplyConventions<ICollectionConvention,ICollectionMappingProvider>(collectionMappingProvider);
        }

        /// <summary>Applies property conventions to <paramref name="propertyMappingProvider"/>.</summary>
        public void Visit(IPropertyMappingProvider propertyMappingProvider)
        {
            SelectAndApplyConventions<IPropertyConvention,IPropertyMappingProvider>(propertyMappingProvider);
        }

        /// <summary>Applies property and dictionary conventions to <paramref name="dictionaryMappingProvider"/>.</summary>
        public void Visit(IDictionaryMappingProvider dictionaryMappingProvider)
        {
            Visit(dictionaryMappingProvider as IPropertyMappingProvider);
            SelectAndApplyConventions<IDictionaryConvention,IDictionaryMappingProvider>(dictionaryMappingProvider);
        }

        /// <summary>Does nothing for now.</summary>
        public void Visit(IClassMappingProvider classMappingProvider)
        {
        }

        /// <summary>Does nothing for now.</summary>
        public void Visit(IEntityMappingProvider entityMappingProvider)
        {
        }

        private void SelectAndApplyConventions<TConvention,TProvider>(TProvider provider) where TConvention:IConvention<TProvider>
        {
            foreach (var convention in _conventions)
            {
                if (convention is TConvention)
                {
                    TConvention typedConvention=(TConvention)convention;
                    if (typedConvention.ShouldApply((TProvider)provider))
                    {
                        typedConvention.Apply(provider);
                    }
                }
            }
        }
    }
}