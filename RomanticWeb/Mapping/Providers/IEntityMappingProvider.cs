using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Providers
{
    public interface IEntityMappingProvider:IMappingProvider
    {
        Type EntityType { get; }

        IEnumerable<IClassMappingProvider> Classes { get; }

        IEnumerable<IPropertyMappingProvider> Properties { get; }
    }
}