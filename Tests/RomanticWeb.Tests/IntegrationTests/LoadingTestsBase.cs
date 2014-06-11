using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;
using RomanticWeb.TestEntities.LargeDataset;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class LoadingTestsBase:IntegrationTestsBase
    {
        private static readonly EntityId EntityId=new EntityId("http://magi/people/Tomasz");

        [Test]
        public void Should_load_as_best_derived_type_in_inheritance_tree()
        {
            // given
            LoadTestFile("InheritingEntities.trig");

            // when
            var entity=EntityContext.Load<IAgent>("http://magi/people/Tomasz");

            // then
            entity.Should().BeAssignableTo<IPerson>();
        }

        [Test]
        public void Should_load_entity_as_best_types()
        {
            // given
            LoadTestFile("InheritingEntities.trig");

            // when
            var entity=EntityContext.Load<IEntity>(EntityId);

            // then
            entity.Should().BeAssignableTo<IPerson>();
            entity.Should().BeAssignableTo<IAlsoPerson>();
        }

        [Test]
        public void Entity_should_be_castable_to_multiple_types()
        {
            // given
            LoadTestFile("InheritingEntities.trig");

            // when
            dynamic entity=EntityContext.Load<IEntity>(EntityId);

            // then
            Assert.That(entity is IPerson);
            Assert.That(entity is IAlsoPerson);
            ((IPerson)entity).Name.Should().Be("Tomasz");
            ((IAlsoPerson)entity).LastName.Should().Be("Pluskiewicz");
        }

        [Test]
        public void Should_load_entity_with_all_matching_derived_types()
        {
            // given
            LoadTestFile("EntityWithManyTypes.trig");

            // when
            var entity=EntityContext.Load<IAgent>(EntityId);

            // then
            entity.Should().BeAssignableTo<TestEntities.Foaf.IPerson>();
        }

        [Test]
        public void Should_load_associated_entities_with_all_matching_derived_types()
        {
            // given
            LoadTestFile("EntityWithManyTypes.trig");
            var entity=EntityContext.Load<IPerson>(EntityId);

            // when
            var karol=entity.KnowsOne;

            // then
            karol.Should().BeAssignableTo<TestEntities.Foaf.IPerson>();
        }

        [Test]
        public void Should_change_entity_type_best_matching_derived_type()
        {
            // given
            LoadTestFile("EntityWithManyTypes.trig");
            var entity=EntityContext.Load<ITypedEntity>(EntityId);

            // when
            var agent=entity.AsEntity<IAgent>();

            // then
            agent.Should().BeAssignableTo<TestEntities.Foaf.IPerson>();
        }

        [Test]
        [TestCase(2)]
        [Category("Slow tests")]
        public void Should_list_entities_from_large_dataset_in_a_timely_fashion_way(int maxLoadTime)
        {
            Assert.Inconclusive("This test is for forcing optimizations only. It's supposed to always fail.");
            // given
            LoadTestFile("LargeDataset.nq");
            DateTime startedAt=DateTime.Now;

            // when
            IEnumerable<IProduct> entities=EntityContext.AsQueryable<IProduct>().ToList();

            // then
            TimeSpan testLength=DateTime.Now-startedAt;
            testLength.TotalSeconds.Should().BeLessOrEqualTo(maxLoadTime);
        }

        [Test]
        [TestCase(2)]
        [Category("Slow tests")]
        public void Should_enumerate_entities_from_large_dataset_in_a_timely_fashion_way(int maxLoadTime)
        {
            Assert.Inconclusive("This test is for forcing optimizations only. It's supposed to always fail.");
            // given
            LoadTestFile("LargeDataset.nq");
            IEnumerable<IProduct> entities=EntityContext.AsQueryable<IProduct>().ToList();
            DateTime startedAt=DateTime.Now;

            // when
            foreach (IProduct product in entities)
            {
                string name=product.Name;
                string comments=product.Comments;
                string viscosity=System.String.Join(", ",product.Viscosity.Select(item => System.String.Format("{0}{1}",item.Unit,item.Value)));
                string cureSystem=(product.CureSystem!=null?product.CureSystem.Id.ToString():System.String.Empty);
                string cureTemperature=System.String.Join(", ",product.CureTemperature.Select(item => System.String.Format("{0}{1}",item.Unit,item.Value)));
                string cureTime=System.String.Join(", ",product.CureTime.Select(item => System.String.Format("{0}{1}",item.Unit,item.Value)));
                string durometer=(product.Durometer!=null?System.String.Format("{0}{1}",product.Durometer.Unit,product.Durometer.Value):System.String.Empty);
                string tensile=(product.Tensile!=null?System.String.Format("{0}{1}",product.Tensile.Unit,product.Tensile.Value):System.String.Empty);
                string elongation=(product.Elongation!=null?System.String.Format("{0}{1}",product.Elongation.Unit,product.Elongation.Value):System.String.Empty);
                string tear=(product.Tear!=null?System.String.Format("{0}{1}",product.Tear.Unit,product.Tear.Value):System.String.Empty);
                string rheology=(product.Rheology!=null?System.String.Format("{0}{1}",product.Rheology.Unit,product.Rheology.Value):System.String.Empty);
                string specificGravity=(product.SpecificGravity!=null?product.SpecificGravity.ToString():System.String.Empty);
                string industry=(product.Industry??System.String.Empty).ToString();
                string grade=(product.Grade??System.String.Empty).ToString();
                string productCategory=(product.ProductCategory!=null?System.String.Join(", ",product.ProductCategory):System.String.Empty);
                string msdsFile=System.String.Join(", ",product.MsdsFile.Select(item => item.Id.ToString()));
                string function=System.String.Join(", ",product.Function.Select(item => item.ToString()));
            }

            // then
            TimeSpan testLength=DateTime.Now-startedAt;
            testLength.TotalSeconds.Should().BeLessOrEqualTo(maxLoadTime);
        }

        [Test]
        [TestCase("mailto:RBall_Consult@msn.com")]
        public void Should_select_entities_by_sub_query(string userId)
        {
            // given
            LoadTestFile("LargeDataset.nq");

            IUser user=EntityContext.Load<IEntity>(new EntityId(new Uri(userId))).AsEntity<IUser>();

            // when
            IList<IProduct> products=(from product in EntityContext.AsQueryable<IProduct>()
                                      where user.FavoriteProduct.Contains(product)
                                      select product).ToList();

            // then
            Assert.That(products.Count,Is.Not.EqualTo(0));
        }

        [Test]
        [Repeat(10)]
        [TestCase("SCV",2)]
        public void Should_return_limited_number_of_entities_from_large_dataset(string searchString,int expectedCount)
        {
            // given
            LoadTestFile("LargeDataset.nq");

            // when
            IList<IProduct> products=EntityContext.AsQueryable<IProduct>()
                .Where(item => item.Name.ToLower().Contains(searchString.ToLower()))
                .Take(2)
                .ToList();

            // then
            Assert.That(products.Count,Is.EqualTo(expectedCount));
        }

        [Test]
        [TestCase("SCV",10)]
        public void Should_return_count_of_entities(string searchString,int expectedCount)
        {
            // given
            LoadTestFile("LargeDataset.nq");

            // when
            int count=EntityContext.AsQueryable<IProduct>().Count(item => item.Name.ToLower().Contains(searchString.ToLower()));

            // then
            Assert.That(count,Is.EqualTo(expectedCount));
        }

        [Test]
        [TestCase("Northrop Grumman")]
        public void Should_return_filtered_entities(params string[] groups)
        {
            // given
            LoadTestFile("LargeDataset.nq");

            // when
            IList<IProduct> products=(from product in EntityContext.AsQueryable<IProduct>()
                                      from userGroup in groups
                                      where (product.Group.Contains(userGroup))||(!product.Group.Any())
                                      select product).ToList();

            // then
            Assert.That(products.Count,Is.Not.EqualTo(0));
        }

        [Test]
        [TestCase("http://chem.com/product/R32-2186","http://chem.com/vocab/cureSystem")]
        public void Should_read_entity_predicates(string productId,string predicateUri)
        {
            // given
            LoadTestFile("LargeDataset.nq");

            // when
            IProduct product=EntityContext.Load<IProduct>(new EntityId(productId));

            // then
            Assert.That(product.Predicates().Count(),Is.Not.EqualTo(0));
            Assert.That(product.Predicate(new Uri(predicateUri)),Is.Not.Null);
        }

        [Test]
        [TestCase("http://chem.com/product/R32-2186","http://chem.com/vocab/viscosity")]
        public void Should_read_blank_node(string productId,string predicateUri)
        {
            // given
            LoadTestFile("LargeDataset.nq");

            // when
            IProduct product=EntityContext.Load<IProduct>(new EntityId(productId));

            // then
            Assert.That(product.Predicate(new Uri(predicateUri)),Is.InstanceOf<IQuantitativeFloatProperty>());
            Assert.That(product.Viscosity.FirstOrDefault(),Is.InstanceOf<IQuantitativeFloatProperty>());
        }

        [Test]
        public void Should_find_entities_with_subquery()
        {
            LoadTestFile("LargeDataset.nq");
            EntityId searched=new EntityId("http://chem.com/vocab/AdhereSealPromoteAdhesion");
            IEnumerable<IProduct> products=from product in EntityContext.AsQueryable<IProduct>()
                                           from func in product.Function
                                           where func==searched
                                           select product;
            Assert.That(products.Count(),Is.Not.EqualTo(0));
        }

        [Test]
        public void Select_with_multiple_combined_statements()
        {
            LoadTestFile("LargeDataset.nq");
            IQueryable<IProduct> result=null;
            var fUri=new EntityId("http://chem.com/vocab/PotEncapsulate");
            result=from product in EntityContext.AsQueryable<IProduct>()
                   from func in product.Function
                   where func==fUri
                   select product;

            var iUri=new EntityId("http://chem.com/vocab/LifeSciences");
            result=from product in result??EntityContext.AsQueryable<IProduct>()
                   where product.Industry==iUri
                   select product;

            IEnumerable<IProduct> products=result.ToList();
            Assert.That(products.Count(),Is.Not.EqualTo(0));
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }
    }
}