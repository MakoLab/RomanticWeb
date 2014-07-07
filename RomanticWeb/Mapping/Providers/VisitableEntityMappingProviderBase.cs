using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    internal abstract class VisitableEntityMappingProviderBase : IEntityMappingProvider
    {
        public abstract Type EntityType { get; }

        public abstract IEnumerable<IClassMappingProvider> Classes { get; }

        public abstract IEnumerable<IPropertyMappingProvider> Properties { get; }

        public void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);

            foreach (var property in Properties)
            {
                property.Accept(mappingProviderVisitor);
            }

            foreach (var classProvider in Classes)
            {
                classProvider.Accept(mappingProviderVisitor);
            }
        }
    }
}