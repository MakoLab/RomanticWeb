using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Tests.Helpers;

namespace RomanticWeb.Tests.IntegrationTests.InMemory
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
                if (_store == null)
                {
                    _store = new FileTripleStore(filePath);
                }

                return _store;
            }
        }

        protected override void ChildSetup()
        {
            base.ChildSetup();
            filePath = Path.Combine(AppDomain.CurrentDomain.GetApplicationStoragePath(), "test.trig");
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

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            Store.LoadTestFile(fileName);
        }

        protected override IEntitySource CreateEntitySource()
        {
            return new TripleStoreAdapter(Store);
        }

        protected override void ChildTeardown()
        {
            _store = null;
        }

        protected override void AsserGraphIntDataSource(Uri graphUri)
        {
            File.ReadAllText(filePath).Should().MatchRegex(System.String.Format("<{0}> {{(.|\n)*}}", graphUri));
            File.ReadAllText(filePath).Should().MatchRegex(System.String.Format("<{0}> <http://xmlns.com/foaf/0.1/primaryTopic> <{0}>", graphUri));
        }
    }
}