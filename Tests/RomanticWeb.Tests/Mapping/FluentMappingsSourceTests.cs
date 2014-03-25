using System.Collections.Generic;
using NUnit.Framework;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.TestEntities.FluentMappings;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class FluentMappingsSourceTests:MappingSourceTests
    {
        protected override IEnumerable<IMappingProviderSource> CreateMappingSources()
        {
            yield return new FluentMappingsSource(typeof(AnimalMap).Assembly);
        }
    }
}