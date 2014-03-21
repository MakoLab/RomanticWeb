using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.MixedMappings;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class InheritanceTreeProviderTests
    {
        private IOntologyProvider _ontology;

        [SetUp]
        public void Setup()
        {
            _ontology=new Mock<IOntologyProvider>().Object;
        }

        [Test]
        public void Should_ignore_overriden_parent_properties()
        {
            // given
            PropertyInfo property=new TestPropertyInfo(GetType());
            var child=new
                          {
                              EntityType=typeof(IDerivedLevel2),
                              Properties=CreateProperty(property,new Uri("urn:in:child")).AsEnumerable(),
                          }.ActLike<IEntityMappingProvider>();
            var parent=new
                           {
                               EntityType=typeof(IDerived),
                               Properties=CreateProperty(property,new Uri("urn:in:parent")).AsEnumerable()
                           }.ActLike<IEntityMappingProvider>();

            // when
            var provider=new InheritanceTreeProvider(child,parent.AsEnumerable());

            // then
            provider.Properties.Should().HaveCount(1);
            provider.Properties.Single().GetTerm(_ontology).Should().Be(new Uri("urn:in:child"));
        }

        private IPropertyMappingProvider CreateProperty(PropertyInfo property, Uri uri)
        {
            return new
                       {
                           PropertyInfo=property,
                           GetTerm=new Func<IOntologyProvider,Uri>(provider => uri)
                       }.ActLike<IPropertyMappingProvider>();
        }
    }
}