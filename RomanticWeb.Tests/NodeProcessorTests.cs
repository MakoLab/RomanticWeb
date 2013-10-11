using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Tests.Helpers;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class NodeProcessorTests
    {
        private Mock<IEntityContext> _entityContext;
        private Mock<IEntityStore> _tripleSource;

        private Mock<IEntityStore> _entityStore;

        [SetUp]
        public void Setup()
        {
            _tripleSource=new Mock<IEntityStore>(MockBehavior.Strict);
            _entityContext=new Mock<IEntityContext>(MockBehavior.Strict);
            _entityStore=new Mock<IEntityStore>(MockBehavior.Strict);
        }

        [TearDown]
        public void Teardown()
        {
            _tripleSource.VerifyAll();
            _entityContext.VerifyAll();
            _entityStore.VerifyAll();
        }

        [Test]
        public void Unless_otherwise_specified_should_convert_URI_nodes_to_Entites()
        {
            // given
            var converter=CreateProcessor();
            var predicate=new Uri("urn:some:predicate");
            var objects = Nodes.Create(10).Uris().GetNodes();
            _entityContext.Setup(ctx => ctx.Create(It.IsAny<EntityId>())).Returns((EntityId id) => new Entity(id));

            // when
            converter.ProcessNodes(predicate,objects).ToList();

            // then
            _entityContext.Verify(ctx=>ctx.Create(It.IsAny<EntityId>()),Times.Exactly(10));
        }

        [Test]
        public void Unless_otherwise_specified_should_convert_blank_nodes_to_Entites()
        {
            // given
            var converter = CreateProcessor();
            var predicate = new Uri("urn:some:predicate");
            var objects = Nodes.Create(10).Blanks().GetNodes();
            _entityContext.Setup(ctx => ctx.Create(It.IsAny<EntityId>())).Returns((EntityId id) => new Entity(id));

            // when
            converter.ProcessNodes(predicate, objects).ToList();

            // then
            _entityContext.Verify(ctx => ctx.Create(It.IsAny<EntityId>()), Times.Exactly(10));
        }

        [Test]
        public void If_no_suitable_converter_is_present_should_convert_typed_literal_nodes_to_string()
        {
            // given
            var intConverter = new Mock<ILiteralNodeConverter>(MockBehavior.Strict);
            intConverter.Setup(c => c.CanConvert(new Uri("http://www.w3.org/2001/XMLSchema#int"))).Returns(false);
            var processor = CreateProcessor(intConverter.Object);
            var predicate = new Uri("urn:some:predicate");
            var objects = Nodes.Create(1)
                               .Literals()
                               .WithDatatype(new Uri("http://www.w3.org/2001/XMLSchema#token"))
                               .WithValues(i => string.Format("literal value {0}", i))
                               .GetNodes();

            // when
            var list = processor.ProcessNodes(predicate, objects).ToList();

            // then
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.InstanceOf<string>());
            Assert.That(list[0], Is.EqualTo("literal value 0"));
            intConverter.VerifyAll();
        }

        [Test]
        public void Should_convert_typed_literal_nodes_using_compatible_node_converter()
        {
            // given
            var intConverter=new Mock<ILiteralNodeConverter>(MockBehavior.Strict);
            intConverter.Setup(c => c.Convert(It.IsAny<Node>())).Returns(5);
            intConverter.Setup(c => c.CanConvert(new Uri("http://www.w3.org/2001/XMLSchema#int"))).Returns(true);
            var processor = CreateProcessor(intConverter.Object);
            var predicate = new Uri("urn:some:predicate");
            var objects = Nodes.Create(1)
                               .Literals()
                               .WithDatatype(new Uri("http://www.w3.org/2001/XMLSchema#int"))
                               .GetNodes();

            // when
            var list = processor.ProcessNodes(predicate, objects).ToList();

            // then
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(5));
            intConverter.VerifyAll();
        }

        private NodeProcessor CreateProcessor(params object[] converters)
        {
            return new NodeProcessor(_entityContext.Object,_entityStore.Object)
                       {
                           Converters=converters.OfType<ILiteralNodeConverter>(),
                           ComplexTypeConverters=converters.OfType<IComplexTypeConverter>()
                       };
        }
    }
}