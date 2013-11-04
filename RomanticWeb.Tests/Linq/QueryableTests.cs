using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Linq
{
    [TestFixture]
    public class QueryableTests
    {
        private Queryable<IPerson> persons;
        private Mock<IEntityStore> _entityStore;
        private Mock<IEntitySource> _entitySource;
        private Mock<IEntityContext> _entityContext;
        private IMappingsRepository _mappings;
        private IOntologyProvider _ontologies;

        [SetUp]
        public void SetUp()
        {
            _mappings=new TestMappingsRepository(new NamedGraphsPersonMapping());
            _ontologies=new TestOntologyProvider();
            _entitySource=new Mock<IEntitySource>(MockBehavior.Strict);
            
            _entityStore=new Mock<IEntityStore>(MockBehavior.Strict);
            _entityStore.Setup(s => s.AssertEntity(It.IsAny<EntityId>(), It.IsAny<IEnumerable<EntityTriple>>()));

            _entityContext=new Mock<IEntityContext>(MockBehavior.Strict);
            _entityContext.Setup(context => context.Load<IPerson>(It.IsAny<EntityId>(),false)).Returns((EntityId id, bool b) => CreatePersonEntity(id));
            _entityContext.Setup(context => context.Store).Returns(_entityStore.Object);
            
            persons = new Queryable<IPerson>(_entityContext.Object,_entitySource.Object, _mappings, _ontologies);
        }

        [TearDown]
        public void Teardown()
        {
            _entitySource.VerifyAll();
        }

        [Test]
        public void Should_assert_triples_resulting_from_query()
        {
            // given
            var query = from p in persons
                        where p.FirstName.Substring(2,1)=="A"
                        select p;
            _entitySource.Setup(e => e.ExecuteEntityQuery(It.IsAny<SparqlQuery>()))
                         .Returns(GetSamplePersonTriples(5));

            // when
            var result = query.ToList();

            // then
            Assert.That(result, Has.Count.EqualTo(5));
            _entityStore.Verify(store => store.AssertEntity(It.IsAny<EntityId>(), It.Is<IEnumerable<EntityTriple>>(t=>t.Count()==10)), Times.Exactly(5));
        }

        protected IEnumerable<EntityTriple> GetSamplePersonTriples(int count)
        {
            const string IdFormat="http://magi/test/person/{0}";
            return from i in Enumerable.Range(1,count)
                   from j in Enumerable.Range(1,10)
                   let id=new EntityId(string.Format(IdFormat,i))
                   let s = Node.ForUri(id.Uri)
                   let p = Node.ForUri(new Uri(string.Format("http://magi/onto/predicate/{0}", j)))
                   let o=Node.ForUri(new Uri(string.Format("http://magi/onto/object/{0}",j)))
                   select new EntityTriple(id,s,p,o);
        }

        private static IPerson CreatePersonEntity(EntityId id)
        {
            return new { Id = id }.ActLike<IPerson>();
        }
    }
}