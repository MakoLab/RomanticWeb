using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Linq.Model;
using RomanticWeb.Mapping;
using RomanticWeb.Model;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Linq
{
    [TestFixture]
    public class QueryableTests
    {
        private EntityQueryable<IPerson> persons;
        private Mock<IEntityStore> _entityStore;
        private Mock<IEntitySource> _entitySource;
        private Mock<IEntityContext> _entityContext;
        private IMappingsRepository _mappings;
        private Mock<IBaseUriSelectionPolicy> _baseUriSelectionPolicy;

        [SetUp]
        public void SetUp()
        {
            _mappings=new TestMappingsRepository(new NamedGraphsPersonMapping());
            _baseUriSelectionPolicy=new Mock<IBaseUriSelectionPolicy>();
            _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri("http://test/"));
            _entitySource=new Mock<IEntitySource>(MockBehavior.Strict);
            
            _entityStore=new Mock<IEntityStore>(MockBehavior.Strict);
            _entityStore.Setup(s => s.AssertEntity(It.IsAny<EntityId>(), It.IsAny<IEnumerable<EntityQuad>>()));

            _entityContext=new Mock<IEntityContext>(MockBehavior.Strict);
            _entityContext.Setup(context => context.Load<IPerson>(It.IsAny<EntityId>())).Returns((EntityId id) => CreatePersonEntity(id));
            _entityContext.Setup(context => context.Store).Returns(_entityStore.Object);
            _entityContext.SetupGet(context => context.Mappings).Returns(_mappings);
            _entityContext.SetupGet(context => context.BaseUriSelector).Returns(_baseUriSelectionPolicy.Object);

            persons=new EntityQueryable<IPerson>(_entityContext.Object,_entitySource.Object,_mappings,_baseUriSelectionPolicy.Object);
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
            _entitySource.Setup(e => e.ExecuteEntityQuery(It.IsAny<Query>())).Returns(GetSamplePersonTriples(5));
            var query=from p in persons
                        where p.FirstName.Substring(2,1)=="A"
                        select p;

            // when
            var result=query.ToList();

            // then
            Assert.That(result, Has.Count.EqualTo(5));
            _entityStore.Verify(store => store.AssertEntity(It.IsAny<EntityId>(), It.Is<IEnumerable<EntityQuad>>(t=>t.Count()==10)), Times.Exactly(5));
        }

        protected IEnumerable<EntityQuad> GetSamplePersonTriples(int count)
        {
            const string IdFormat="http://magi/test/person/{0}";
            return from i in Enumerable.Range(1,count)
                   from j in Enumerable.Range(1,10)
                   let id=new EntityId(string.Format(IdFormat,i))
                   let s=Node.ForUri(id.Uri)
                   let p=Node.ForUri(new Uri(string.Format("http://magi/onto/predicate/{0}",j)))
                   let o=Node.ForUri(new Uri(string.Format("http://magi/onto/object/{0}",j)))
                   select new EntityQuad(id,s,p,o);
        }

        private static IPerson CreatePersonEntity(EntityId id)
        {
            return new { Id=id }.ActLike<IPerson>();
        }

        internal class NamedGraphsPersonMapping:TestEntityMapping<IPerson>
        {
            public NamedGraphsPersonMapping()
            {
                Property("FirstName", Vocabularies.Foaf.givenName, typeof(string), new StringConverter());
                Property("LastName", Vocabularies.Foaf.familyName, typeof(string), new StringConverter());
                Property("Homepage", Vocabularies.Foaf.homepage, typeof(Uri), new DefaultUriConverter());
                Property("FirstName", Vocabularies.Foaf.givenName, typeof(string), new StringConverter());
                Property("Friend", Vocabularies.Foaf.knows, typeof(IPerson), new AsEntityConverter<IPerson>());
                Property("Age", Vocabularies.Foaf.age, typeof(int), new IntegerConverter());
                Collection("Friends", new Uri("http://xmlns.com/foaf/0.1/friends"), typeof(IEnumerable<IPerson>), new AsEntityConverter<IPerson>());

                Class(Vocabularies.Foaf.Person);

                RdfList("Interests", Vocabularies.Foaf.interest, typeof(IEnumerable<string>));
            }
        }
    }
}