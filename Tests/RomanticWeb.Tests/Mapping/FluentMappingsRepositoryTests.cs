using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class FluentMappingsRepositoryTests : MappingRepositoryTests
    {
        protected override IMappingsRepository CreateMappingsRepository()
        {
            return new FluentMappingsRepository(typeof(IAnimal).Assembly);
        }
    }
}