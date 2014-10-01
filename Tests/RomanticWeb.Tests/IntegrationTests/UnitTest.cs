using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RomanticWeb;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.LargeDataset;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.IntegrationTests;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace TestProject
{
    [TestClass]
    public class UnitTest : IntegrationTestsBase
    {
        private TripleStore _store;

        protected override ITripleStore Store
        {
            get
            {
                if (_store == null)
                {
                    _store = new TripleStore();
                }

                return _store;
            }
        }

        [TestInitialize]
        public void Setup()
        {
            typeof(IntegrationTestsBase).GetMethod("Setup", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Invoke(this, null);
        }

        [TestCleanup]
        public void Teardown()
        {
            typeof(IntegrationTestsBase).GetMethod("Teardown", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Invoke(this, null);
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            Store.LoadTestFile(fileName);
        }

        protected override void ChildTeardown()
        {
            _store = null;
        }

        [TestMethod]
        public void Should_enumerate_entities_from_large_dataset_in_a_timely_fashion_way()
        {
            ////Assert.Inconclusive("This test is for forcing optimizations only. It's supposed to always fail.");

            // given
            LoadTestFile("LargeDataset.nq");
            IEnumerable<IProduct> entities = EntityContext.AsQueryable<IProduct>().ToList();
            DateTime startedAt = DateTime.Now;

            // when
            foreach (IProduct product in entities)
            {
                string name = product.Name;
                string comments = product.Comments;
                string viscosity = (product.Viscosity != null ? System.String.Join(", ", product.Viscosity.Viscosity.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value))) : System.String.Empty);
                string cureSystem = (product.CureSystem != null ? product.CureSystem.ToString() : System.String.Empty);
                string cureTemperature = System.String.Join(", ", product.CureTemperature.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value)));
                string cureTime = System.String.Join(", ", product.CureTime.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value)));
                string durometer = System.String.Join(", ", product.Durometer.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value)));
                string tensile = System.String.Join(", ", product.Tensile.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value)));
                string elongation = System.String.Join(", ", product.Elongation.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value)));
                string tear = (product.Tear != null ? System.String.Format("{0}{1}", product.Tear.Unit, product.Tear.Value) : System.String.Empty);
                string rheology = System.String.Join(", ", product.Rheology.Select(item => System.String.Format("{0}{1}", item.Unit, item.Value)));
                string specificGravity = System.String.Join(", ", product.SpecificGravity);
                string industry = (product.Industry ?? System.String.Empty).ToString();
                string grade = System.String.Join(", ", product.Grade.Select(item => item.ToString()));
                string productCategory = System.String.Join(", ", product.ProductCategory);
                string msdsFile = System.String.Join(", ", product.MsdsFile.Select(item => item.Id.ToString()));
                string function = System.String.Join(", ", product.Function.Select(item => item.ToString()));
            }

            // then
            TimeSpan testLength = DateTime.Now - startedAt;
            testLength.TotalSeconds.Should().BeLessOrEqualTo(2);
        }
    }
}