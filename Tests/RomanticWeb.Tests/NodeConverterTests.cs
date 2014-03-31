////using System;
////using System.Collections.Generic;
////using System.Linq;
////using FluentAssertions;
////using ImpromptuInterface;
////using Moq;
////using NUnit.Framework;
////using RomanticWeb.Converters;
////using RomanticWeb.Entities;
////using RomanticWeb.Mapping.Model;
////using RomanticWeb.Model;
////using RomanticWeb.Tests.Helpers;
////using VDS.RDF;

////namespace RomanticWeb.Tests
////{
////    [TestFixture]
////    public class NodeConverterTests
////    {
////        private Mock<IEntityContext> _entityContext;
////        private Mock<IEntityStore> _tripleSource;
////        private Mock<IEntityStore> _entityStore;

////        [SetUp]
////        public void Setup()
////        {
////            _tripleSource=new Mock<IEntityStore>(MockBehavior.Strict);
////            _entityContext=new Mock<IEntityContext>(MockBehavior.Strict);
////            _entityStore=new Mock<IEntityStore>(MockBehavior.Strict);
////        }

////        [TearDown]
////        public void Teardown()
////        {
////            _tripleSource.VerifyAll();
////            _entityContext.VerifyAll();
////            _entityStore.VerifyAll();
////        }

////        [Test]
////        public void Unless_otherwise_specified_should_convert_URI_nodes_to_Entites()
////        {
////            // given
////            var converter=CreateConverter();
////            var objects = Nodes.Create(10).Uris().GetNodes();
////            _entityContext.Setup(ctx => ctx.Load<IEntity>(It.IsAny<EntityId>(), false)).Returns((EntityId id, bool b) => new Entity(id));

////            // when
////            converter.ConvertNodes(objects,null).ToList();

////            // then
////            _entityContext.Verify(ctx => ctx.Load<IEntity>(It.IsAny<EntityId>(), false), Times.Exactly(10));
////        }

////        [Test]
////        public void Unless_otherwise_specified_should_convert_blank_nodes_to_Entites()
////        {
////            // given
////            var converter = CreateConverter();
////            var objects = Nodes.Create(10).Blanks().GetNodes(new EntityId("urn:test:node"));
////            _entityContext.Setup(ctx => ctx.Load<IEntity>(It.IsAny<EntityId>(), false)).Returns((EntityId id, bool b) => new Entity(id));

////            // when
////            converter.ConvertNodes(objects,null).ToList();

////            // then
////            _entityContext.Verify(ctx => ctx.Load<IEntity>(It.IsAny<EntityId>(), false), Times.Exactly(10));
////        }

////        [Test]
////        public void If_no_suitable_converter_is_present_should_convert_typed_literal_nodes_to_string()
////        {
////            // given
////            var intConverter = new Mock<ILiteralNodeConverter>(MockBehavior.Strict);
////            intConverter.Setup(c => c.CanConvert(It.IsAny<Node>()))
////                        .Returns(new LiteralConversionMatch());
////            var processor = CreateConverter(intConverter.Object);
////            var objects = Nodes.Create(1)
////                               .Literals()
////                               .WithDatatype(new Uri("http://www.w3.org/2001/XMLSchema#token"))
////                               .WithValues(i => string.Format("literal value {0}", i))
////                               .GetNodes();

////            // when
////            var list = processor.ConvertNodes(objects,null).ToList();

////            // then
////            Assert.That(list, Has.Count.EqualTo(1));
////            Assert.That(list[0], Is.InstanceOf<string>());
////            Assert.That(list[0], Is.EqualTo("literal value 0"));
////            intConverter.VerifyAll();
////        }

////        [Test]
////        public void Should_convert_typed_literal_nodes_using_compatible_node_converter()
////        {
////            // given
////            var intConverter=new Mock<ILiteralNodeConverter>(MockBehavior.Strict);
////            intConverter.Setup(c => c.Convert(It.IsAny<Node>())).Returns(5);
////            intConverter.Setup(c => c.CanConvert(It.IsAny<Node>()))
////                        .Returns((Node n) => CreateSuccesfulMatch());
////            var processor = CreateConverter(intConverter.Object);
////            var objects = Nodes.Create(1)
////                               .Literals()
////                               .WithDatatype(new Uri("http://www.w3.org/2001/XMLSchema#int"))
////                               .GetNodes();

////            // when
////            var list = processor.ConvertNodes(objects,null).ToList();

////            // then
////            Assert.That(list, Has.Count.EqualTo(1));
////            Assert.That(list[0], Is.EqualTo(5));
////            intConverter.VerifyAll();
////        }

////        [TestCase(typeof(IEnumerable<int>))]
////        [TestCase(typeof(IEnumerable<string>))]
////        [TestCase(typeof(IList<string>))]
////        public void Should_convert_rdf_list_root_to_Entity(Type collectionType)
////        {
////            // given
////            var mapping = new
////            {
////                ReturnType=collectionType,
////                StoreAs=StoreAs.RdfList
////            }.ActLike<ICollectionMapping>();
////            var entity=new { }.ActLike<IEntity>();
////            _entityContext.Setup(c => c.Load<IEntity>(It.IsAny<EntityId>(),false)).Returns(entity);
////            var intConverter=new Mock<IUriNodeConverter>(MockBehavior.Strict);
////            intConverter.Setup(c => c.Convert(entity, mapping)).Returns(entity);
////            intConverter.Setup(c => c.CanConvert(entity, mapping)).Returns(true);

////            // when
////            var processor=CreateConverter(intConverter.Object);
////            var objects=Nodes.Create(1).Uris().GetNodes();
////            var list=processor.ConvertNodes(objects,mapping).ToList();

////            // then
////            Assert.That(list,Has.Count.EqualTo(1));
////            Assert.That(list,Contains.Item(entity));
////            intConverter.VerifyAll();
////        }

////        [TestCase(typeof(IEnumerable<int>))]
////        [TestCase(typeof(IEnumerable<string>))]
////        [TestCase(typeof(IList<string>))]
////        public void Should_not_convert_rdf_list_elements(Type collectionType)
////        {
////            // given
////            var mapping = new
////            {
////                ReturnType = collectionType,
////                StoreAs = StoreAs.RdfList
////            }.ActLike<IPropertyMapping>();

////            // when
////            var processor=CreateConverter();
////            var objects=Nodes.Create(1).Literals().GetNodes();
////            var list=processor.ConvertNodes(objects,mapping).ToList();

////            // then
////            list.Should().HaveCount(1);
////            list.Should().OnlyContain(el => el is string);
////        }

////        [Test]
////        public void Should_convert_Uri_to_string()
////        {
////            // given
////            var node=Node.ForUri(new Uri("urn:uri:node"));
////            var property = new
////                             {
////                                 ReturnType=typeof(string),
////                                 StoreAs=StoreAs.Undefined
////                             }.ActLike<IPropertyMapping>();

////            // when
////            var processor=CreateConverter();
////            var converted=processor.ConvertNodes(node.AsEnumerable(),property);

////            // then
////            converted.Should().Equal(new object[] { "urn:uri:node" });
////        }

////        private static LiteralConversionMatch CreateSuccesfulMatch()
////        {
////            return new LiteralConversionMatch
////                       {
////                           DatatypeMatches=MatchResult.ExactMatch,
////                           LiteralFormatMatches=MatchResult.ExactMatch
////                       };
////        }

////        private FallbackNodeConverter CreateConverter(params object[] converters)
////        {
////            var literalConverters=converters.OfType<ILiteralNodeConverter>().ToList();
////            var complexConverters=converters.OfType<IUriNodeConverter>().ToList();
////            var catalog=new Mock<IConverterCatalog>();
////            catalog.Setup(cc => cc.UriNodeConverters).Returns(complexConverters);
////            catalog.Setup(cc => cc.LiteralNodeConverters).Returns(literalConverters);
////            return new FallbackNodeConverter(_entityContext.Object,catalog.Object);
////        }
////    }
////}