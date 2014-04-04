using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ImpromptuInterface;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.TestEntities.Foaf;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityTypeMatcherTests
    {
        private EntityTypeMatcher _entityTypeMatcher;
        private ITypedEntity _entity;

        [SetUp]
        public void Setup()
        {
            _entity=new TypedEntity();
            _entityTypeMatcher=new EntityTypeMatcher();
        }

        [Test]
        public void Should_return_requested_type_if_not_mapped()
        {
            // when
            var type=_entityTypeMatcher.GetMostDerivedMappedType(_entity,typeof(IAgent));

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_requested_type_if_it_is_only_mapped()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));

            // when
            var type=_entityTypeMatcher.GetMostDerivedMappedType(_entity,typeof(IAgent));

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
            var type=_entityTypeMatcher.GetMostDerivedMappedType(_entity,typeof(IAgent));

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
            var type=_entityTypeMatcher.GetMostDerivedMappedType(_entity,typeof(IAgent));

            // then
            type.Should().Be<IAgent>();
        }

        [Test]
        public void Should_return_correct_subtype_when_base_type_explicitly_stated()
        {
            // given
            Visit(CreateMapping<IAgent>(Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IOrganization>(Vocabularies.Foaf.Organization,Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IGroup>(Vocabularies.Foaf.Group,Vocabularies.Foaf.Agent));
            Visit(CreateMapping<IPerson>(Vocabularies.Foaf.Person,Vocabularies.Foaf.Agent));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Person));
            _entity.Types.Add(new EntityId(Vocabularies.Foaf.Agent));

            // when
            var type=_entityTypeMatcher.GetMostDerivedMappedType(_entity,typeof(IAgent));

            // then
            type.Should().Be<IPerson>();
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
            return new { Uri=uri }.ActLike<IClassMapping>();
        }

        private void Visit(IEntityMapping mapping)
        {
            _entityTypeMatcher.Visit(mapping);

            foreach (var classMapping in mapping.Classes)
            {
                _entityTypeMatcher.Visit(classMapping);
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