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

namespace RomanticWeb.Tests.IntegrationTests.FileBased
{
    [TestFixture]
    public class NamedGraphMappingTests : NamedGraphMappingTestsBase
    {
        private FileTripleStore _store;
        private string filePath;

        protected FileTripleStore Store
        {
            get
            {
                if (this._store == null)
                {
                    this._store = new FileTripleStore(this.filePath);
                }

                return this._store;
            }
        }

        [Test]
        public void Should_store_blank_nodes_correctly()
        {
            // given
            var entity = this.EntityContext.Create<IPerson>(new EntityId("urn:t:p"));
            var friend = this.EntityContext.Create<IPerson>(entity.CreateBlankId());
            entity.Friend = friend;
            friend.FirstName = "D";

            // when
            this.EntityContext.Commit();

            // then
            this._store.Should().MatchAsk(b => b.Subject(new Uri("urn:t:p")).PredicateUri(Foaf.knows).Object("blank")
                                                .Subject("blank").PredicateUri(Foaf.givenName).ObjectLiteral("D", Xsd.String));
        }

        protected override RomanticWeb.Mapping.Sources.IMappingProviderSource SetupMappings()
        {
            return new TestMappingSource(new PersonMap());
        }

        protected override void ChildSetup()
        {
            base.ChildSetup();
            this.filePath = Path.Combine(AppDomain.CurrentDomain.GetApplicationStoragePath(), "test.trig");
            if (!Directory.Exists(AppDomain.CurrentDomain.GetApplicationStoragePath()))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.GetApplicationStoragePath());
            }

            if (File.Exists(this.filePath))
            {
                File.Delete(this.filePath);
            }

            File.Create(this.filePath).Close();
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            this.Store.LoadTestFile(fileName);
        }

        protected override IEntitySource CreateEntitySource()
        {
            return new TripleStoreAdapter(this.Store);
        }

        protected override void ChildTeardown()
        {
            this._store = null;
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