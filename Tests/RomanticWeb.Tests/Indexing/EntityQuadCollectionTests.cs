using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Indexing
{
    [TestFixture]
    public class EntityQuadCollectionTests
    {
        private EntityQuadCollection _quads;

        [SetUp]
        protected void Setup()
        {
            _quads=new EntityQuadCollection();
        }

        [Test]
        [TestCase("http://test/subject1")]
        public void Should_add_entity_quad(string entityId)
        {
            // When
            EntityQuad quad=CreateEntityQuad(entityId);
            _quads.Add(quad);
            int indexOf=((IList<EntityQuad>)_quads.Quads).IndexOf(quad);

            // Then
            indexOf.Should().NotBe(-1);
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Should().NotBeNull();
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].StartAt.Should().Be(indexOf);
        }

        [Test]
        [TestCase("http://test/subject1",3)]
        public void Should_add_many_entity_quads_for_same_entity(string entityId,int count)
        {
            // When
            for (int index=0; index<count; index++)
            {
                EntityQuad quad=CreateEntityQuad(entityId);
                _quads.Add(quad);
            }

            // Then
            ((IList<EntityQuad>)_quads.Quads).Count.Should().Be(count);
            for (int index=0; index<count; index++)
            {
                _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Should().NotBeNull();
                _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].StartAt.Should().Be(0);
                _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Length.Should().Be(count);
            }
        }

        [Test]
        [TestCase("http://test/subject1","http://test/subject2","http://test/subject3")]
        public void Should_add_many_entity_quads_for_different_entities(params string[] entityIds)
        {
            // When
            foreach (string entityId in entityIds)
            {
                EntityQuad quad=CreateEntityQuad(entityId);
                _quads.Add(quad);
            }

            // Then
            ((IList<EntityQuad>)_quads.Quads).Count.Should().Be(3);
            int index=0;
            foreach (string entityId in entityIds)
            {
                _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Should().NotBeNull();
                _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].StartAt.Should().Be(index);
                _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Length.Should().Be(1);
                index++;
            }
        }

        [Test]
        [TestCase("http://test/subject1")]
        public void Should_remove_entity_quad(string entityId)
        {
            // Given
            EntityQuad quad=CreateEntityQuad(entityId);
            _quads.Add(quad);

            // When
            _quads.Remove(quad);
            int indexOf=((IList<EntityQuad>)_quads.Quads).IndexOf(quad);

            // Then
            indexOf.Should().Be(-1);
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Should().BeNull();
        }

        [Test]
        [TestCase("http://test/subject2")]
        public void Should_remove_entity_quad_from_many_other_entities(string entityId)
        {
            // Given
            _quads.Add(CreateEntityQuad("http://test/subject1"));
            _quads.Add(CreateEntityQuad("http://test/subject1"));
            _quads.Add(CreateEntityQuad("http://test/subject1"));
            _quads.Add(CreateEntityQuad("http://test/subject3"));
            _quads.Add(CreateEntityQuad("http://test/subject3"));
            _quads.Add(CreateEntityQuad("http://test/subject3"));
            EntityQuad quad=CreateEntityQuad(entityId);
            _quads.Add(quad);

            // When
            _quads.Remove(quad);
            int indexOf=((IList<EntityQuad>)_quads.Quads).IndexOf(quad);

            // Then
            indexOf.Should().Be(-1);
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Should().BeNull();
        }


        [Test]
        [TestCase("http://test/subject2")]
        public void Should_remove_entity_quad_from_many_various_entities(string entityId)
        {
            // Given
            _quads.Add(CreateEntityQuad("http://test/subject1"));
            _quads.Add(CreateEntityQuad("http://test/subject1"));
            _quads.Add(CreateEntityQuad("http://test/subject1"));
            _quads.Add(CreateEntityQuad(entityId));
            _quads.Add(CreateEntityQuad(entityId));
            _quads.Add(CreateEntityQuad("http://test/subject3"));
            _quads.Add(CreateEntityQuad("http://test/subject3"));
            _quads.Add(CreateEntityQuad("http://test/subject3"));
            EntityQuad quad=CreateEntityQuad(entityId);
            _quads.Add(quad);

            // When
            _quads.Remove(quad);
            int indexOf=((IList<EntityQuad>)_quads.Quads).IndexOf(quad);

            // Then
            indexOf.Should().Be(-1);
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Should().NotBeNull();
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].StartAt.Should().Be(3);
            _quads.Subjects[entityId,IndexCollection<string>.FirstPossible].Length.Should().Be(2);
        }

        private EntityQuad CreateEntityQuad(string entityId)
        {
            return new EntityQuad(
                new EntityId(new Uri(entityId)),
                Node.ForUri(new Uri(entityId)),
                Node.ForUri(new Uri("http://test/predicate"+Guid.NewGuid().ToString())),
                Node.ForLiteral(Guid.NewGuid().ToString(),Xsd.String),
                Node.ForUri(new Uri(entityId.Replace("http","graph"))));
        }
    }
}