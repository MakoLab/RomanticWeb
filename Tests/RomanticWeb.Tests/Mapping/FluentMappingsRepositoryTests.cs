using NUnit.Framework;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.TestEntities.FluentMappings;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class FluentMappingsRepositoryTests : MappingRepositoryTests
    {
        protected override IMappingSource CreateMappingSource()
        {
            return new FluentMappingsSource(typeof(AnimalMap).Assembly);
        }
    }
}