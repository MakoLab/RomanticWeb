using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class AttributeMappingsRepositoryTests : MappingRepositoryTests
    {
        protected override IMappingSource CreateMappingSource()
        {
            return new AttributeMappingsSource(typeof(IAnimal).Assembly);
        }
    }
}