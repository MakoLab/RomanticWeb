using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Providers
{
    public interface ITermMappingProvider:IMappingProvider
    {
        Func<IOntologyProvider,Uri> GetTerm { get; set; }
    }
}