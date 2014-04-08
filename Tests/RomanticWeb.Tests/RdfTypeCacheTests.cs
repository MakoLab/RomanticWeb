using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ImpromptuInterface;
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
        private static readonly dynamic New=Builder.New();
        private RdfTypeCache _rdfTypeCache;
        private ITypedEntity _entity;

        [SetUp]
        public void Setup()
        {
            _entity=new TypedEntity();
            _rdfTypeCache=new RdfTypeCache();
        }

        [Test]
        public void Should_return_requested_type_if_not_mapped()
        {
            // when
            var type=_rdfTypeCache.GetMostDerivedMappedTypes(_entity,typeof(IAgent)).Single();

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_requested_type_if_it_is_only_mapped()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));

            // when
            var type=_rdfTypeCache.GetMostDerivedMappedTypes(_entity,typeof(IAgent)).Single();

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_derived_type_if_requested_parent_but_entity_is_appropriately_typed()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IPerson>(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));

            // when
            var type=_rdfTypeCache.GetMostDerivedMappedTypes(_entity,typeof(IAgent)).Single();

            // then
            type.Should().Be<IPerson>();
        }

        [Test]
        public void Should_not_return_unrelated_type_mapped_to_same_class_URI()
        {
            // given
            Visit(CreateMapping<IAgent>());
            Visit(CreateMapping<TestEntities.IPerson>(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));

            // when
            var type=_rdfTypeCache.GetMostDerivedMappedTypes(_entity,typeof(IAgent)).Single();

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_correct_subtype_when_base_type_explicitly_stated()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IOrganization>(Vocabularies.Foaf.Organization));
            Visit(CreateMapping<IGroup>(Vocabularies.Foaf.Group));
            Visit(CreateMapping<IPerson>(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));

            // when
            var type = _rdfTypeCache.GetMostDerivedMappedTypes(_entity, typeof(IAgent)).Single();

            // then
            type.Should().Be<IPerson>();
        }

        [Test]
        public void Should_return_correct_subtype_when_additional_unrelated_types_present()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IPerson>(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));
            _entity.Types.Add(new EntityId(new Uri("urn:other:type")));

            // when
            var type=_rdfTypeCache.GetMostDerivedMappedTypes(_entity,typeof(IAgent)).Single();

            // then
            type.Should().Be<IPerson>();
        }

        [Test]
        public void Should_return_multiple_matching_most_derived_types()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IPerson>(Vocabularies.Foaf.Person));
            Visit(CreateMapping<IAlsoPerson>(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));

            // when
            var type=_rdfTypeCache.GetMostDerivedMappedTypes(_entity,typeof(IAgent)).ToList();

            // then
            type.Should().Contain(typeof(IPerson));
            type.Should().Contain(typeof(IAlsoPerson));
            type.Should().NotContain(typeof(IAgent));
        }

        private static IEntityMapping CreateMapping<T>(params Uri[] classUris)
        {
            var classMappings=classUris.Select(CreateClassMapping);
            return new
                       {
                           EntityType=typeof(T),
                           Classes=classMappings
                       }.ActLike<IEntityMapping>();
        }

        private static IClassMapping CreateClassMapping(Uri uri)
        {
            return New.ExpandoObject(
                        IsInherited: false,
                        IsMatch: new Func<IEnumerable<Uri>,bool>(uris => uris.Contains(uri)))
                      .ActLike<IClassMapping>();
        }

        private void Visit(IEntityMapping mapping)
        {
            _rdfTypeCache.Visit(mapping);

            foreach (var classMapping in mapping.Classes)
            {
                _rdfTypeCache.Visit(classMapping);
            }
        }

        private class TypedEntity:ITypedEntity
        {
            public TypedEntity()
            {
                Types=new List<EntityId>();
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

            public ICollection<EntityId> Types { get; set; }
        }
    }
}