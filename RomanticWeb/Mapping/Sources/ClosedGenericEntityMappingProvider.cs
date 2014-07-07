using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Sources
{
    internal class ClosedGenericEntityMappingProvider : IEntityMappingProvider
    {
        private readonly Type _closedGenericEntityType;
        private readonly OpenGenericEntityMappingCollector _collector;

        public ClosedGenericEntityMappingProvider(IEntityMappingProvider openGenericProvider, params Type[] typeArguments)
        {
            _closedGenericEntityType = openGenericProvider.EntityType.MakeGenericType(typeArguments);
            _collector = new OpenGenericEntityMappingCollector(typeArguments);
            openGenericProvider.Accept(_collector);
        }

        public Type EntityType
        {
            get
            {
                return _closedGenericEntityType;
            }
        }

        public IEnumerable<IClassMappingProvider> Classes
        {
            get
            {
                return _collector.ClassMappingProviders;
            }
        }

        public IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                return _collector.PropertyMappingProviders;
            }
        }

        public void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            var @this = ((IEntityMappingProvider)this);
            mappingProviderVisitor.Visit(this);

            foreach (var property in @this.Properties)
            {
                property.Accept(mappingProviderVisitor);
            }

            foreach (var classProvider in @this.Classes)
            {
                classProvider.Accept(mappingProviderVisitor);
            }
        }
    }
}