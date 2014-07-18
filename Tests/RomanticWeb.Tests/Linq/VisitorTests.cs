using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Sparql;
using RomanticWeb.Mapping;
using RomanticWeb.Model;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Linq
{
    [TestFixture]
    public class VisitorTests
    {
        private Mock<IEntitySource> _entitySource;
        private EntityQueryable<IPerson> _persons;
        private Mock<IEntityStore> _entityStore;
        private Mock<IEntityContext> _entityContext;
        private IMappingsRepository _mappings;
        private Mock<IBaseUriSelectionPolicy> _baseUriSelectionPolicy;
        private Tuple<IQueryable<IPerson>, string, string, string, string, string, string>[] _testQueries;

        [SetUp]
        public void Setup()
        {
            _mappings = new TestMappingsRepository(new QueryableTests.NamedGraphsPersonMapping());
            _baseUriSelectionPolicy = new Mock<IBaseUriSelectionPolicy>();
            _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri("http://test/"));
            _entitySource = new Mock<IEntitySource>(MockBehavior.Strict);
            _entityStore = new Mock<IEntityStore>(MockBehavior.Strict);
            _entityStore.Setup(store => store.AssertEntity(It.IsAny<EntityId>(), It.IsAny<IEnumerable<EntityQuad>>()));

            _entityContext = new Mock<IEntityContext>(MockBehavior.Strict);
            _entityContext.Setup(context => context.Load<IPerson>(It.IsAny<EntityId>())).Returns((EntityId id) => CreatePersonEntity(id));
            _entityContext.Setup(context => context.Store).Returns(_entityStore.Object);
            _entityContext.SetupGet(context => context.Mappings).Returns(_mappings);
            _entityContext.SetupGet(context => context.BaseUriSelector).Returns(_baseUriSelectionPolicy.Object);

            _persons = new EntityQueryable<IPerson>(_entityContext.Object, _entitySource.Object, _mappings, _baseUriSelectionPolicy.Object);
            _testQueries = GetTestQueries();
        }

        [Test]
        public void Test_correctness_of_query_selecting_all_persons()
        {
            Test_correctness_of_query(0);
        }

        [Test]
        public void Test_correctness_of_query_selecting_person_by_its_name()
        {
            Test_correctness_of_query(1);
        }

        [Test]
        public void Test_correctness_of_query_selecting_person_that_has_a_friend_with_specific_name()
        {
            Test_correctness_of_query(2);
        }

        [Test]
        public void Test_correctness_of_query_selecting_person_that_has_a_friend_with_specific_name_alternative()
        {
            Test_correctness_of_query(3);
        }

        [Test]
        public void Test_correctness_of_query_selecting_person_that_has_a_friend_that_has_any_friend_with_name_starting_with_Ka()
        {
            Test_correctness_of_query(4);
        }

        [Test]
        public void Test_correctness_of_query_asking_for_any_person()
        {
            Test_correctness_of_query_asking(5);
        }

        [Test]
        public void Test_correctness_of_query_asking_for_count_of_persons()
        {
            Test_correctness_of_scalar_query(6);
        }

        private void Test_correctness_of_query_asking(int queryIndex)
        {
            Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string> query = _testQueries[queryIndex];
            var computedCommandText = string.Empty;
            _entitySource.Setup(e => e.ExecuteAskQuery(It.IsAny<Query>())).Returns<Query>(model =>
            {
                computedCommandText = VisitModel(model).CommandText;
                return true;
            });

            query.Item1.Any();
            computedCommandText = Regex.Replace(computedCommandText, @"\s+", string.Empty).Trim();
            var expectedText = Regex.Replace(query.Item2, @"\s+", string.Empty).Trim();
            Assert.That(computedCommandText, Is.EqualTo(expectedText));
        }

        private void Test_correctness_of_scalar_query(int queryIndex)
        {
            Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string> query = _testQueries[queryIndex];
            string computedCommandText = string.Empty;
            _entitySource.Setup(e => e.ExecuteScalarQuery(It.IsAny<Query>())).Returns<Query>(model =>
            {
                computedCommandText = VisitModel(model).CommandText;
                return 1;
            });

            query.Item1.Count();
            computedCommandText = Regex.Replace(computedCommandText, @"\s+", string.Empty).Trim();
            var expectedText = Regex.Replace(query.Item2, @"\s+", string.Empty).Trim();
            Assert.That(computedCommandText, Is.EqualTo(expectedText));
        }

        private void Test_correctness_of_query(int queryIndex)
        {
            Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string> query = _testQueries[queryIndex];
            string computedCommandText = string.Empty;
            string computedMetaGraphVariableName = string.Empty;
            string computedEntityVariableName = string.Empty;
            string computedSubjectVariableName = string.Empty;
            string computedPredicateVariableName = string.Empty;
            string computedObjectVariableName = string.Empty;
            _entitySource.Setup(e => e.ExecuteEntityQuery(It.IsAny<Query>())).Returns<Query>(model =>
            {
                GenericSparqlQueryVisitor visitor = VisitModel(model);
                computedCommandText = visitor.CommandText;
                computedMetaGraphVariableName = visitor.Variables.MetaGraph;
                computedEntityVariableName = visitor.Variables.Entity;
                computedSubjectVariableName = visitor.Variables.Subject;
                computedPredicateVariableName = visitor.Variables.Predicate;
                computedObjectVariableName = visitor.Variables.Object;
                return GetSamplePersonTriples(5);
            });

            query.Item1.ToList();
            computedCommandText = Regex.Replace(computedCommandText, @"\s+", string.Empty).Trim();
            var expectedText = Regex.Replace(query.Item2, @"\s+", string.Empty).Trim();
            Assert.That(computedCommandText, Is.EqualTo(expectedText));
            Assert.That(computedMetaGraphVariableName, Is.EqualTo(query.Item3));
            Assert.That(computedEntityVariableName, Is.EqualTo(query.Item4));
            Assert.That(computedSubjectVariableName, Is.EqualTo(query.Item5));
            Assert.That(computedPredicateVariableName, Is.EqualTo(query.Item6));
            Assert.That(computedObjectVariableName, Is.EqualTo(query.Item7));
        }

        protected GenericSparqlQueryVisitor VisitModel(Query queryModel)
        {
            GenericSparqlQueryVisitor visitor = new GenericSparqlQueryVisitor();
            visitor.MetaGraphUri = new Uri("http://app.magi/graphs");
            visitor.VisitQuery(queryModel);
            return visitor;
        }

        protected Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>[] GetTestQueries()
        {
            return new[] 
            {
                new Tuple<IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons select person,
                    Resourcer.Resource.AsString("Queries.SelectAll.rq"),
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"),
                new Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons where person.FirstName == "Karol" select person,
                    Resourcer.Resource.AsString("Queries.SelectByName.rq"),
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"),
                new Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons where person.Friends.Any(friend => friend.FirstName == "Karol") select person,
                    Resourcer.Resource.AsString("Queries.SelectFriendByNameWithSubquery.rq"),
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"),
                new Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons from friend in person.Friends where friend.FirstName == "Karol" select person,
                    Resourcer.Resource.AsString("Queries.SelectFriendByName.rq"),
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"),
                new Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons where person.Friend.Friends.Any(friend => Regex.IsMatch(friend.FirstName, "Ka.*")) select person,
                    Resourcer.Resource.AsString("Queries.SelectEntityComplexSubquery.rq"),
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"),
                new Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons select person,
                    Resourcer.Resource.AsString("Queries.Ask.rq"),
                    null,
                    null,
                    null,
                    null,
                    null),
                new Tuple<System.Linq.IQueryable<IPerson>, string, string, string, string, string, string>(
                    from person in _persons select person,
                    Resourcer.Resource.AsString("Queries.SelectCountDistinct.rq"),
                    null,
                    null,
                    null,
                    null,
                    null)
            };
        }

        protected IEnumerable<EntityQuad> GetSamplePersonTriples(int count)
        {
            const string IdFormat = "http://magi/test/person/{0}";
            return from i in Enumerable.Range(1, count)
                   from j in Enumerable.Range(1, 10)
                   let id = new EntityId(string.Format(IdFormat, i))
                   let s = Node.ForUri(id.Uri)
                   let p = Node.ForUri(new Uri(string.Format("http://magi/onto/predicate/{0}", j)))
                   let o = Node.ForUri(new Uri(string.Format("http://magi/onto/object/{0}", j)))
                   select new EntityQuad(id, s, p, o);
        }

        private static IPerson CreatePersonEntity(EntityId id)
        {
            return new { Id = id }.ActLike<IPerson>();
        }
    }
}