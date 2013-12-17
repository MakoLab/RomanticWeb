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
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.IntegrationTests;
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
        private IOntologyProvider _ontologies;
        private Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>[] _testQueries;

        [SetUp]
        public void Setup()
        {
            _mappings=new TestMappingsRepository(new NamedGraphsPersonMapping());
            _ontologies=new TestOntologyProvider();
            _mappings.RebuildMappings(new MappingContext(_ontologies,new DefaultGraphSelector()));
            _entitySource=new Mock<IEntitySource>(MockBehavior.Strict);
            _entityStore=new Mock<IEntityStore>(MockBehavior.Strict);
            _entityStore.Setup(store => store.AssertEntity(It.IsAny<EntityId>(),It.IsAny<IEnumerable<EntityQuad>>()));

            _entityContext=new Mock<IEntityContext>(MockBehavior.Strict);
            _entityContext.Setup(context => context.Load<IPerson>(It.IsAny<EntityId>(),false)).Returns((EntityId id,bool checkIfExists) => CreatePersonEntity(id));
            _entityContext.Setup(context => context.Store).Returns(_entityStore.Object);

            _persons=new EntityQueryable<IPerson>(_entityContext.Object,_entitySource.Object,_mappings);
            _testQueries=GetTestQueries();
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
            Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string> query=_testQueries[queryIndex];
            string computedCommandText="";
            _entitySource.Setup(e => e.ExecuteAskQuery(It.IsAny<Query>())).Returns<Query>(model =>
            {
                computedCommandText=VisitModel(model).CommandText;
                return true;
            });

            query.Item1.Any();
            computedCommandText=Regex.Replace(Regex.Replace(computedCommandText.Replace("\r",""),"[\n\t]"," ")," {2,}"," ").Trim();
            Assert.That(computedCommandText,Is.EqualTo(query.Item2));
        }

        private void Test_correctness_of_scalar_query(int queryIndex)
        {
            Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string> query=_testQueries[queryIndex];
            string computedCommandText="";
            _entitySource.Setup(e => e.ExecuteScalarQuery(It.IsAny<Query>())).Returns<Query>(model =>
            {
                computedCommandText=VisitModel(model).CommandText;
                return 1;
            });

            query.Item1.Count();
            computedCommandText=Regex.Replace(Regex.Replace(computedCommandText.Replace("\r",""),"[\n\t]"," ")," {2,}"," ").Trim();
            Assert.That(computedCommandText,Is.EqualTo(query.Item2));
        }

        private void Test_correctness_of_query(int queryIndex)
        {
            Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string> query=_testQueries[queryIndex];
            string computedCommandText="";
            string computedMetaGraphVariableName="";
            string computedEntityVariableName="";
            string computedSubjectVariableName="";
            string computedPredicateVariableName="";
            string computedObjectVariableName="";
            _entitySource.Setup(e => e.ExecuteEntityQuery(It.IsAny<Query>())).Returns<Query>(model =>
            {
                GenericSparqlQueryVisitor visitor=VisitModel(model);
                computedCommandText=visitor.CommandText;
                computedMetaGraphVariableName=visitor.Variables.MetaGraph;
                computedEntityVariableName=visitor.Variables.Entity;
                computedSubjectVariableName=visitor.Variables.Subject;
                computedPredicateVariableName=visitor.Variables.Predicate;
                computedObjectVariableName=visitor.Variables.Object;
                return GetSamplePersonTriples(5);
            });

            query.Item1.ToList();
            computedCommandText=Regex.Replace(Regex.Replace(computedCommandText.Replace("\r",""),"[\n\t]"," ")," {2,}"," ").Trim();
            Assert.That(computedCommandText,Is.EqualTo(query.Item2));
            Assert.That(computedMetaGraphVariableName,Is.EqualTo(query.Item3));
            Assert.That(computedEntityVariableName,Is.EqualTo(query.Item4));
            Assert.That(computedSubjectVariableName,Is.EqualTo(query.Item5));
            Assert.That(computedPredicateVariableName,Is.EqualTo(query.Item6));
            Assert.That(computedObjectVariableName,Is.EqualTo(query.Item7));
        }

        protected GenericSparqlQueryVisitor VisitModel(Query queryModel)
        {
            GenericSparqlQueryVisitor visitor=new GenericSparqlQueryVisitor();
            visitor.MetaGraphUri=new Uri("http://app.magi/graphs");
            visitor.VisitQuery(queryModel);
            return visitor;
        }

        protected Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>[] GetTestQueries()
        {
            return new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>[] {
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "SELECT ?s ?p ?o ?Gperson0 ?person0 "+
                    "WHERE { "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                    "}",
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"
                ),
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons where person.FirstName=="Karol" select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "SELECT ?s ?p ?o ?Gperson0 ?person0 "+
                    "WHERE { "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                            "?person0 <http://xmlns.com/foaf/0.1/givenName> ?firstName0 . "+
                            "FILTER (?firstName0=\"Karol\"^^xsd:string) "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                    "}",
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"
                ),
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons where person.Friends.Any(friend => friend.FirstName=="Karol") select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "SELECT ?s ?p ?o ?Gperson0 ?person0 "+
                    "WHERE { "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                            "?person0 <http://xmlns.com/foaf/0.1/friends> ?friend0 . "+
                            "FILTER ("+
                                "EXISTS { "+
                                    "SELECT ?friend0 "+
                                    "WHERE { "+
                                        "GRAPH ?Gfriend0 { "+
                                            "?friend0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                                            "?friend0 <http://xmlns.com/foaf/0.1/givenName> ?firstName0 . "+
                                            "FILTER (?firstName0=\"Karol\"^^xsd:string) "+
                                        "} "+
                                        "GRAPH <http://app.magi/graphs> { "+
                                            "?Gfriend0 <http://xmlns.com/foaf/0.1/primaryTopic> ?friend0 . "+
                                        "} "+
                                    "} "+
                                "} "+
                            ") "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                    "}",
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"
                ),
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons from friend in person.Friends where friend.FirstName=="Karol" select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "SELECT ?s ?p ?o ?Gperson0 ?person0 "+
                    "WHERE { "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                            "?person0 <http://xmlns.com/foaf/0.1/friends> ?friend0 . "+
                            "FILTER (?firstName0=\"Karol\"^^xsd:string) " +
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                        "GRAPH ?Gfriend0 { "+
                            "?friend0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                            "?friend0 <http://xmlns.com/foaf/0.1/givenName> ?firstName0 . "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gfriend0 <http://xmlns.com/foaf/0.1/primaryTopic> ?friend0 . "+
                        "} "+
                    "}",
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"
                ),
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons where person.Friend.Friends.Any(friend => Regex.IsMatch(friend.FirstName,"Ka.*")) select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "SELECT ?s ?p ?o ?Gperson0 ?person0 "+
                    "WHERE { "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                            "?person0 <http://xmlns.com/foaf/0.1/knows> ?friend1 . "+
                            "FILTER ("+
                                "EXISTS { "+
                                    "SELECT ?friend0 "+
                                    "WHERE { "+
                                        "GRAPH ?Gfriend0 { "+
                                            "?friend0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                                            "?friend0 <http://xmlns.com/foaf/0.1/givenName> ?firstName0 . "+
                                            "FILTER (REGEX(?firstName0,\"Ka.*\"^^xsd:string)) "+
                                        "} "+
                                        "GRAPH <http://app.magi/graphs> { "+
                                            "?Gfriend0 <http://xmlns.com/foaf/0.1/primaryTopic> ?friend0 . "+
                                        "} "+
                                        "GRAPH ?Gfriend1 { "+
                                            "?friend1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                                            "?friend1 <http://xmlns.com/foaf/0.1/friends> ?friend0 . "+
                                        "} "+
                                        "GRAPH <http://app.magi/graphs> { "+
                                            "?Gfriend1 <http://xmlns.com/foaf/0.1/primaryTopic> ?friend1 . "+
                                        "} "+
                                    "} "+
                                "} "+
                            ") "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                    "}",
                    "Gperson0",
                    "person0",
                    "s",
                    "p",
                    "o"
                ),
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "ASK "+
                    "{ "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                    "}",
                    null,
                    null,
                    null,
                    null,
                    null
                ),
                new Tuple<System.Linq.IQueryable<IPerson>,string,string,string,string,string,string>(
                    from person in _persons select person,
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> "+
                    "SELECT COUNT(DISTINCT(?s)) AS ?personCount0 "+
                    "WHERE { "+
                        "GRAPH ?Gperson0 { "+
                            "?s ?p ?o . "+
                            "?person0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . "+
                        "} "+
                        "GRAPH <http://app.magi/graphs> { "+
                            "?Gperson0 <http://xmlns.com/foaf/0.1/primaryTopic> ?person0 . "+
                        "} "+
                    "}",
                    null,
                    null,
                    null,
                    null,
                    null
                )
            };
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
    }
}