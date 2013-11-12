using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class AttributeMappingsRepositoryTests : MappingRepositoryTests
    {
        protected override IMappingsRepository CreateMappingsRepository()
        {
            return new AttributeMappingsRepository(typeof(IAnimal).Assembly);
        }
    }
}