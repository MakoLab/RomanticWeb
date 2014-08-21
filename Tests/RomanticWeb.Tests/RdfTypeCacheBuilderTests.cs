using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.TestEntities.Foaf;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class RdfTypeCacheBuilderTests
    {
        private static readonly dynamic New = Builder.New();
        private RdfTypeCacheBuilder _builder;
        private Mock<IRdfTypeCache> _cache;

        [SetUp]
        public void Setup()
        {
            _cache = new Mock<IRdfTypeCache>();
            _builder = new RdfTypeCacheBuilder(_cache.Object);
        }

        [Test]
        public void Accepting_builder_should_populate_cache()
        {
            // given
            var mapping = CreateMapping<IAgent>(Vocabularies.Foaf.Agent);

            // when
            mapping.Accept(_builder);

            // then
            _cache.Verify(c => c.Add(typeof(IAgent), It.Is<IList<IClassMapping>>(m => m.Count == 1)));
        }

        private static IEntityMapping CreateMapping<T>(params Uri[] classUris)
        {
            var classMappings = classUris.Select(CreateClassMapping);
            dynamic mapping = New.ExpandoObject(
                EntityType: typeof(T),
                Classes: classMappings);
            mapping.Accept = new Action<IMappingModelVisitor>(visitor => Accept(mapping.ActLike<IEntityMapping>(), visitor));

            return mapping.ActLike<IEntityMapping>();
        }

        private static IClassMapping CreateClassMapping(Uri uri)
        {
            return New.ExpandoObject(
                IsInherited: false,
                IsMatch: new Func<IEnumerable<Uri>, bool>(uris => uris.Contains(uri)))
                      .ActLike<IClassMapping>();
        }

        private static void Accept(IEntityMapping mapping, IMappingModelVisitor visitor)
        {
            visitor.Visit(mapping);
            foreach (var classMapping in mapping.Classes)
            {
                visitor.Visit(classMapping);
            }
        }
    }
}