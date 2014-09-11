using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ImpromptuInterface.Dynamic;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.TestEntities.Foaf;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class RdfTypeCacheTests
    {
        private static readonly dynamic New = Builder.New();
        private RdfTypeCache _rdfTypeCache;
        private ITypedEntityWritable _entity;

        [SetUp]
        public void Setup()
        {
            _entity = new TypedEntity();
            _rdfTypeCache = new RdfTypeCache();
        }

        [Test]
        public void Should_return_requested_type_if_not_mapped()
        {
            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).Single();

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_requested_type_if_it_is_only_mapped()
        {
            // given
            _rdfTypeCache.Add(typeof(IAgent), CreateClassMappings(Vocabularies.Foaf.Agent));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).Single();

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_derived_type_if_requested_parent_but_entity_is_appropriately_typed()
        {
            // given
            _rdfTypeCache.Add(typeof(IAgent), CreateClassMappings(Vocabularies.Foaf.Agent));
            _rdfTypeCache.Add(typeof(IPerson), CreateClassMappings(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).Single();

            // then
            type.Should().Be<IPerson>();
        }

        [Test]
        public void Should_not_return_unrelated_type_mapped_to_same_class_URI()
        {
            // given
            _rdfTypeCache.Add(typeof(IAgent), CreateClassMappings(Vocabularies.Foaf.Agent));
            _rdfTypeCache.Add(typeof(TestEntities.IPerson), CreateClassMappings(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).Single();

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_correct_subtype_when_base_type_explicitly_stated()
        {
            // given
            _rdfTypeCache.Add(typeof(IAgent), CreateClassMappings(Vocabularies.Foaf.Agent));
            _rdfTypeCache.Add(typeof(IOrganization), CreateClassMappings(Vocabularies.Foaf.Organization));
            _rdfTypeCache.Add(typeof(IGroup), CreateClassMappings(Vocabularies.Foaf.Group));
            _rdfTypeCache.Add(typeof(IPerson), CreateClassMappings(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).Single();

            // then
            type.Should().Be<IPerson>();
        }

        [Test]
        public void Should_return_correct_subtype_when_additional_unrelated_types_present()
        {
            // given
            _rdfTypeCache.Add(typeof(IAgent), CreateClassMappings(Vocabularies.Foaf.Agent));
            _rdfTypeCache.Add(typeof(IPerson), CreateClassMappings(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));
            _entity.Types.Add(new EntityId(new Uri("urn:other:type")));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).Single();

            // then
            type.Should().Be<IPerson>();
        }

        [Test]
        public void Should_return_multiple_matching_most_derived_types()
        {
            // given
            _rdfTypeCache.Add(typeof(IAgent), CreateClassMappings(Vocabularies.Foaf.Agent));
            _rdfTypeCache.Add(typeof(IPerson), CreateClassMappings(Vocabularies.Foaf.Person));
            _rdfTypeCache.Add(typeof(IAlsoPerson), CreateClassMappings(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity.Types.Select(item => item.Uri), typeof(IAgent)).ToList();

            // then
            type.Should().Contain(typeof(IPerson));
            type.Should().Contain(typeof(IAlsoPerson));
            type.Should().NotContain(typeof(IAgent));
        }

        private static IClassMapping CreateClassMapping(Uri uri)
        {
            return New.ExpandoObject(
                        IsInherited: false,
                        IsMatch: new Func<IEnumerable<Uri>, bool>(uris => uris.Contains(uri)))
                      .ActLike<IClassMapping>();
        }

        private static IList<IClassMapping> CreateClassMappings(params Uri[] uris)
        {
            return (from uri in uris select CreateClassMapping(uri)).ToList();
        }

        private class TypedEntity : ITypedEntityWritable
        {
            public TypedEntity()
            {
                Types = new List<EntityId>();
            }

            public EntityId Id
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            public IEntityContext Context
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            public IEnumerable<EntityId> Types { get; private set; }

            IList<EntityId> ITypedEntityWritable.Types
            {
                get
                {
                    return Types.ToList();
                }

                set
                {
                    Types = value;
                }
            }
        }
    }
}