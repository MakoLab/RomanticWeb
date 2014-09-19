using System;
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

namespace RomanticWeb.Tests.IntegrationTests.FileBased
{
    [TestFixture]
    public class NamedGraphMappingTests : NamedGraphMappingTestsBase
    {
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.GetApplicationStoragePath(), "test.trig");

        [Test]
        public void Should_store_blank_nodes_correctly()
        {
            // given
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

        protected override void ChildSetup()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.GetApplicationStoragePath()))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.GetApplicationStoragePath());
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.Create(filePath).Close();
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
            Console.WriteLine("Creating store");
            return new FileTripleStore(filePath);
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