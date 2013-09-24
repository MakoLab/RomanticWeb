using NUnit.Framework;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityStoreTests
    {
        private EntityStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new EntityStore();
        }
    }
}