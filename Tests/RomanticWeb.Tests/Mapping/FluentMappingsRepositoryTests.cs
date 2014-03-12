using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.TestEntities.FluentMappings;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class FluentMappingsRepositoryTests : MappingRepositoryTests
    {
        protected override IMappingsRepository CreateMappingsRepository()
        {
            return new FluentMappingsRepository(typeof(AnimalMap).Assembly);
        }
    }
}