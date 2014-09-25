using System;
using System.Linq;
using NUnit.Framework;
using RomanticWeb.Tests.Helpers;

namespace RomanticWeb.Tests.IntegrationTests.InMemory
{
    [TestFixture]
    public class WritingTests : WritingTestsBase
    {
        protected override int MetagraphTripleCount
        {
            get
            {
                if (!Store.HasGraph(MetaGraphUri))
                {
                    return 0;
                }
                
                return Store[MetaGraphUri].Triples.Count;
            }
        }

        protected override int AllTriplesCount
        {
            get
            {
                return Store.Triples.Count();
            }
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            Store.LoadTestFile(fileName);
        }
    }
}