using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.TestEntities.LargeDataset;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class NamedGraphMappingTestsBase:IntegrationTestsBase
    {
        [Test]
        public void Should_set_graph_for_new_entities()
        {
            // Given
            Uri entityUri=new Uri("http://test/product");
            IProduct product=EntityContext.Create<IProduct>(new EntityId(entityUri));

            // When
            product.Name="test";
            EntityContext.Commit();

            // Then
            foreach (EntityQuad quad in product.Context.Store.Quads)
            {
                quad.Graph.Should().NotBeNull();
                quad.Graph.Uri.AbsoluteUri.Should().Be(entityUri.AbsoluteUri);
            }

            AsserGraphIntDataSource(entityUri);
        }

        protected abstract void AsserGraphIntDataSource(Uri graphUri);
    }
}