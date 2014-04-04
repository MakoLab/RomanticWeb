﻿using System;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Entities
{
    [TestFixture]
    public class EntityProxyTests
    {
        private readonly EntityId _entityId = new EntityId("urn:test:entity");
        private EntityProxy _entityProxy;
        private Mock<IEntityMapping> _mapping;
        private Mock<IEntityContext> _context;
        private Mock<INamedGraphSelector> _graphSelector;

        [SetUp]
        public void Setup()
        {
            _mapping=new Mock<IEntityMapping>(MockBehavior.Strict);
            _context=new Mock<IEntityContext>(MockBehavior.Strict);
            _graphSelector=new Mock<INamedGraphSelector>();

            _context.Setup(c => c.Store).Returns(new EntityStore());
            _context.Setup(c => c.GraphSelector).Returns(_graphSelector.Object);
            _context.Setup(c => c.InitializeEnitity(It.IsAny<IEntity>()));
            _graphSelector.Setup(g => g.SelectGraph(It.IsAny<EntityId>(),It.IsAny<IEntityMapping>(),It.IsAny<IPropertyMapping>()))
                          .Returns(new Uri("urn:default:graph"));

            var entity = new Entity(_entityId, _context.Object);
            _entityProxy=new EntityProxy(entity,_mapping.Object,new TestTransformerCatalog());
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Should_allow_overriding_parameters_for_selecting_named_graph()
        {
            // given
            var idToUse=new EntityId("urn:actual:id");
            var entityMappingToUse=ImpromptuInterface.Dynamic.Builder.New().ActLike<IEntityMapping>();
            IPropertyMapping mappingToUse=new Mock<IPropertyMapping>().Object;
            var propertyMapping=new Mock<IPropertyMapping>().Object;
            _mapping.Setup(m => m.PropertyFor("property"))
                    .Returns(propertyMapping);

            // when
            _entityProxy.OverrideGraphSelection(new OverridingGraphSelector(idToUse,entityMappingToUse,mappingToUse));
            Impromptu.InvokeGet(_entityProxy,"property");

            // then
            _graphSelector.Verify(c => c.SelectGraph(idToUse,entityMappingToUse,mappingToUse),Times.Once);
            _graphSelector.Verify(c => c.SelectGraph(_entityId,_mapping.Object,propertyMapping),Times.Never);
        }

        [Test]
        public void Should_retrieve_named_graph_when_getting_property()
        {
            // given
            var propertyMapping=new Mock<IPropertyMapping>().Object;
            _mapping.Setup(m => m.PropertyFor("property"))
                    .Returns(propertyMapping);

            // when
            Impromptu.InvokeGet(_entityProxy, "property");

            // then
            _graphSelector.Verify(c => c.SelectGraph(_entityId,_mapping.Object,propertyMapping),Times.Once);
        }
    }
}