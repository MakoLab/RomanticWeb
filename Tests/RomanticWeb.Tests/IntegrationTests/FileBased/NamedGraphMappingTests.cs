using System;
using System.Globalization;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Vocabularies;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace RomanticWeb.Tests.IntegrationTests.FileBased
{
    [TestFixture]
    public class NamedGraphMappingTests : NamedGraphMappingTestsBase
    {
        private string filePath;

        [Test]
        public void Should_store_blank_nodes_correctly()
        {
            // given
            EntityContext.CurrentCulture = CultureInfo.InvariantCulture;
            var entity = EntityContext.Create<IPerson>(new EntityId("urn:t:p"));
            var friend = EntityContext.Create<IPerson>(entity.CreateBlankId());
            entity.Friend = friend;
            friend.FirstName = "D";

            // when
            EntityContext.Commit();

            // then
            Store.Should().MatchAsk(b => b.Subject(new Uri("urn:t:p")).PredicateUri(Foaf.knows).Object("blank")
                                          .Subject("blank").PredicateUri(Foaf.givenName).ObjectLiteral("D", Xsd.String));
        }

        protected override RomanticWeb.Mapping.Sources.IMappingProviderSource SetupMappings()
        {
            return new TestMappingSource(new PersonMap());
        }

        protected override void ChildTeardown()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            this.Store.LoadTestFile(fileName);
        }

        protected override ITripleStore CreateTripleStore()
        {
            filePath = Path.GetTempFileName();
            return new FileTripleStore(filePath, new TriGParser(), new TriGWriter());
        }

        protected override void AsserGraphIntDataSource(Uri graphUri)
        {
            File.ReadAllText(this.filePath).Should().MatchRegex(System.String.Format("<{0}> {{(.|\n)*}}", graphUri));
            File.ReadAllText(this.filePath).Should().MatchRegex(System.String.Format("<{0}> <http://xmlns.com/foaf/0.1/primaryTopic> <{0}>", graphUri));
        }

        private class PersonMap : RomanticWeb.Mapping.Fluent.EntityMap<IPerson>
        {
            public PersonMap()
            {
                Property(p => p.Friend).Term.Is(Foaf.knows);
                Property(p => p.FirstName).Term.Is(Foaf.givenName);
            }
        }
    }
}