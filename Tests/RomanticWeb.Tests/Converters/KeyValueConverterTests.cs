using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.Vocabularies;
using VDS.RDF;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture,Ignore,Obsolete]
    public class KeyValueConverterTests
    {
        private static readonly string BaseUri="http://magi/ontology#";
        private IEntityContext entityContext;
        private Mock<IOntologyProvider> magiOntologyProvider;

        [SetUp]
        public void Setup()
        {
            ITripleStore tripleStore=new TripleStore();
            tripleStore.LoadFromEmbeddedResource("RomanticWeb.Tests.TestGraphs.Dictionary.trig, RomanticWeb.Tests");
            TripleStoreAdapter tripleSource=new TripleStoreAdapter(tripleStore);
            magiOntologyProvider=new Mock<IOntologyProvider>();
            magiOntologyProvider.SetupGet(provider => provider.Ontologies).Returns(
                new Ontology[]
                {
                    new Ontology(new NamespaceSpecification("magi",BaseUri),new Class("Element"),new Property("setting"))
                });
            magiOntologyProvider.Setup(provider => provider.ResolveUri(It.IsAny<string>(),It.IsAny<string>())).Returns<string,string>((prefix,termName) => new Uri(BaseUri+termName));
            entityContext=new EntityContextFactory()
                .WithOntology(new DefaultOntologiesProvider())
                .WithOntology(magiOntologyProvider.Object)
                .WithMappings(mappingBuilder =>
                    {
                        mappingBuilder.Attributes.FromAssemblyOf<ITypedEntity>();
                        mappingBuilder.Attributes.FromAssembly(GetType().Assembly);
                    })
                .WithEntitySource(() => tripleSource).CreateContext();
        }

        [Test]
        public void Should_load_entity_with_dictionary()
        {
            IElement element=entityContext.Load<IElement>((EntityId)"http://magi/element/HtmlText");
            Assert.That(element.Settings.Count,Is.EqualTo(2));
            Assert.That(element.Settings.ContainsKey("mode"),Is.EqualTo(true));
            Assert.That(element.Settings["mode"],Is.EqualTo(1));
            Assert.That(element.Settings.ContainsKey("source"),Is.EqualTo(true));
            Assert.That(element.Settings["source"],Is.EqualTo("some text"));
        }

        [Class("magi","Element")]
        public interface IElement:IEntity
        {
            [Collection("magi","setting")]
            IDictionary<string,object> Settings { get; }
        }
    }
}