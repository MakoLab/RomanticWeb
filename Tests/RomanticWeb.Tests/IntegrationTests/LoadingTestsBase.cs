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
        [Timeout(2*1000)]
        public void Should_load_entities_from_large_dataset_in_a_timely_fashion_way()
        {
            // given
            LoadTestFile("LargeDataset.nq");

            // when
            EntityContext.AsQueryable<IProduct>().ToList();
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
            Assert.That(products.Count,Is.EqualTo(expectedCount));
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