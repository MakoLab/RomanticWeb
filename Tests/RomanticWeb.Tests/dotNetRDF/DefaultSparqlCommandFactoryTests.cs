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
        public void Should_create_drop_graph_command_for_GraphDelete()
        {
            // given
            var update = new GraphDelete(EntityId, EntityGraph);

            // when
            var commands = _commandFactory.CreateCommands(update).ToList();

            // then
            commands.Should().HaveCount(2);
            commands[0].Should().BeOfType<DropCommand>();
            commands[0].AffectsSingleGraph.Should().BeTrue();
            commands[0].AffectsGraph(new Uri(EntityGraph)).Should().BeTrue();
            commands[1].Should().BeOfType<DeleteCommand>();
            commands[1].AffectsSingleGraph.Should().BeTrue();
            commands[1].AffectsGraph(MetaGraphUri).Should().BeTrue();
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
            commands[0].Should().BeOfType<ModifyCommand>();
            commands[0].AffectsSingleGraph.Should().BeTrue();
            commands[0].AffectsGraph(new Uri(EntityGraph)).Should().BeTrue();
            commands[1].Should().BeOfType<InsertCommand>();
            commands[1].AffectsSingleGraph.Should().BeTrue();
            commands[1].AffectsGraph(MetaGraphUri).Should().BeTrue();
        }
    }
}