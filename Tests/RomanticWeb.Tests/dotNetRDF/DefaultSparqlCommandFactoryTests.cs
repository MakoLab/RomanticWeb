using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Model;
using RomanticWeb.Updates;
using RomanticWeb.Vocabularies;
using VDS.RDF.Update.Commands;

namespace RomanticWeb.Tests.DotNetRDF
{
    [TestFixture]
    public class DefaultSparqlCommandFactoryTests
    {
        private const string EntityGraph = "urn:entity:graph";
        private const string EntityId = "urn:entity:id";

        private DefaultSparqlCommandFactory _commandFactory;

        private static readonly Uri MetaGraphUri = new Uri("http://example.com/meta.graph");
        private static readonly EntityQuad AQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("A"));
        private static readonly EntityQuad BQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("B"));

        [SetUp]
        public void Setup()
        {
            _commandFactory = new DefaultSparqlCommandFactory(MetaGraphUri);
        }

        [Test]
        public void Should_create_modify_command_for_GraphUpdate()
        {
            // given
            var update = new GraphUpdate(EntityId, EntityGraph, new[] { AQuad }, new[] { BQuad });

            // when
            var commands = _commandFactory.CreateCommands(update).ToList();

            // then
            commands.Should().HaveCount(2);
            commands[0].Should().BeOfType<DeleteCommand>();
            commands[0].AffectsSingleGraph.Should().BeTrue();
            commands[0].AffectsGraph(new Uri(EntityGraph)).Should().BeTrue();
            commands[1].Should().BeOfType<InsertDataCommand>();
        }

        [Test]
        public void Should_create_correct_command_for_graph_update_in_case_of_an_created_blank_node()
        {
            // given
            var update = new GraphUpdate(EntityId, EntityGraph, new EntityQuad[0], new[] { BQuad });

            // when
            var commands = _commandFactory.CreateCommands(update).ToList();

            // then
            commands.Should().HaveCount(1);
            commands[0].Should().BeOfType<InsertDataCommand>();
        }

        [Test]
        public void Should_create_correct_command_for_graph_update_in_case_of_a_deleted_blank_node()
        {
            // given
            var update = new GraphUpdate(EntityId, EntityGraph, new[] { AQuad }, new EntityQuad[0]);

            // when
            var commands = _commandFactory.CreateCommands(update).ToList();

            // then
            commands.Should().HaveCount(2);
            commands[0].Should().BeOfType<DeleteCommand>();
            commands[1].Should().BeOfType<InsertDataCommand>();
            commands[1].AffectsGraph(new Uri(EntityGraph)).Should().BeFalse();
            commands[1].AffectsGraph(MetaGraphUri).Should().BeTrue();
        }
    }
}