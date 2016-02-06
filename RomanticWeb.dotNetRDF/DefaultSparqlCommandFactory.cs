using System;
using System.Collections.Generic;
using System.Linq;
using Resourcer;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Updates;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using VDS.RDF.Update.Commands;
using VDS.RDF.Writing.Formatting;
using Triple = VDS.RDF.Triple;

namespace RomanticWeb.DotNetRDF
{
    internal class DefaultSparqlCommandFactory : ISparqlCommandFactory
    {
        private static readonly string ModifyEntityCommandText = Resource.AsString("Queries.ModifyEntityGraph.ru");
        private static readonly string InsterBlankEntityCommandText = Resource.AsString("Queries.InsertBlankEntityData.ru");
        private static readonly string ReconstructCommandText = Resource.AsString("Queries.ReconstructGraph.ru");
        private static readonly string RemoveReferencesCommandText = Resource.AsString("Queries.RemoveReferences.ru");
        private static readonly string DeleteEntityCommandText = Resource.AsString("Queries.DeleteEntity.ru");

        private readonly Dictionary<Type, Func<dynamic, IEnumerable<SparqlUpdateCommand>>> _commandFactories;
        private readonly Uri _metaGraphUri;
        private readonly SparqlUpdateParser _parser = new SparqlUpdateParser();

        public DefaultSparqlCommandFactory(Uri metaGraphUri)
        {
            _metaGraphUri = metaGraphUri;
            _commandFactories = new Dictionary<Type, Func<dynamic, IEnumerable<SparqlUpdateCommand>>>();
            _commandFactories[typeof(GraphUpdate)] = update => CreateGraphUpdateCommand(update);
            _commandFactories[typeof(GraphReconstruct)] = reconstruct => CreateReconstructCommand(reconstruct);
            _commandFactories[typeof(RemoveReferences)] = remove => CreateRemoveReferencesCommand(remove);
            _commandFactories[typeof(EntityDelete)] = delete => CreateDeleteEntityCommand(delete);
        }

        protected Uri MetaGraphUri
        {
            get
            {
                return _metaGraphUri;
            }
        }

        public IEnumerable<SparqlUpdateCommand> CreateCommands(DatasetChange change)
        {
            var changeType = change.GetType();
            if (_commandFactories.ContainsKey(changeType))
            {
                return _commandFactories[changeType](change);
            }

            throw new ArgumentOutOfRangeException("change", string.Format("Unsupported dataset change type {0}", changeType.Name));
        }

        private static string ConvertTriples(IEnumerable<EntityQuad> removedQuads, INodeFactory factory)
        {
            var formatter = new NTriplesFormatter();
            var quads = from quad in removedQuads
                        let subject = quad.Subject.UnWrapNode(factory)
                        let predicate = quad.Predicate.UnWrapNode(factory)
                        let @object = quad.Object.UnWrapNode(factory)
                        select new Triple(subject, predicate, @object).ToString(formatter);

            return string.Join(Environment.NewLine, quads);
        }

        private IEnumerable<SparqlUpdateCommand> CreateGraphUpdateCommand(GraphUpdate change)
        {
            INodeFactory factory = new NodeFactory();
            var removedTriples = ConvertTriples(change.RemovedQuads, factory);
            var addedTriples = ConvertTriples(change.AddedQuads, factory);

            var format = change.Entity is BlankId ? InsterBlankEntityCommandText : ModifyEntityCommandText;
            var commandText = string.Format(format, removedTriples, addedTriples);
            var commands = new SparqlParameterizedString(commandText);
            commands.SetUri("graph", change.Graph.Uri);

            if (!(change.Entity is BlankId))
            {
                commands.SetUri("metaGraph", MetaGraphUri);
                commands.SetUri("entity", change.Entity.Uri);
            }

            return GetParsedCommands(commands);
        }

        private IEnumerable<SparqlUpdateCommand> CreateReconstructCommand(GraphReconstruct change)
        {
            INodeFactory factory = new NodeFactory();
            var addedTriples = ConvertTriples(change.AddedQuads, factory);

            var commandText = string.Format(ReconstructCommandText, addedTriples);
            var deleteCommands = new SparqlParameterizedString(commandText);
            deleteCommands.SetUri("graph", change.Graph.Uri);
            deleteCommands.SetUri("metaGraph", MetaGraphUri);
            deleteCommands.SetUri("entity", change.Entity.Uri);

            return GetParsedCommands(deleteCommands);
        }

        private IEnumerable<SparqlUpdateCommand> CreateRemoveReferencesCommand(RemoveReferences removeReferences)
        {
            var removeReferenceCommand = new SparqlParameterizedString(RemoveReferencesCommandText);
            removeReferenceCommand.SetUri("reference", removeReferences.Entity.Uri);

            return GetParsedCommands(removeReferenceCommand);
        }

        private IEnumerable<SparqlUpdateCommand> CreateDeleteEntityCommand(EntityDelete deleteEntity)
        {
            var deleteCommand = new SparqlParameterizedString(DeleteEntityCommandText);
            deleteCommand.SetUri("entity", deleteEntity.Entity.Uri);
            deleteCommand.SetUri("metaGraph", MetaGraphUri);

            return GetParsedCommands(deleteCommand);
        } 

        private IEnumerable<SparqlUpdateCommand> GetParsedCommands(SparqlParameterizedString commandString)
        {
            var result = _parser.ParseFromString(commandString).Commands;
            var toBeRemoved = new List<SparqlUpdateCommand>();
            foreach (var command in result)
            {
                var deleteCommand = command as DeleteCommand;
                if (deleteCommand != null)
                {
                    SanitizeDeleteCommand(toBeRemoved, deleteCommand);

                    continue;
                }

                var insertDataCommand = command as InsertDataCommand;
                if (insertDataCommand != null)
                {
                    SanitizeInsertDataCommand(toBeRemoved, insertDataCommand);
                }
            }

            return result.Except(toBeRemoved);
        }

        private void SanitizeDeleteCommand(ICollection<SparqlUpdateCommand> toBeRemoved, DeleteCommand deleteCommand)
        {
            if (!deleteCommand.WherePattern.ChildGraphPatterns.SelectMany(graph => graph.TriplePatterns).Union(deleteCommand.WherePattern.TriplePatterns).Any())
            {
                toBeRemoved.Add(deleteCommand);
            }
        }

        private void SanitizeInsertDataCommand(ICollection<SparqlUpdateCommand> toBeRemoved, InsertDataCommand insertDataCommand)
        {
            for (var index = insertDataCommand.DataPattern.ChildGraphPatterns.Count - 1; index >= 0; index--)
            {
                if (insertDataCommand.DataPattern.ChildGraphPatterns[index].TriplePatterns.Count == 0)
                {
                    insertDataCommand.DataPattern.ChildGraphPatterns.RemoveAt(index);
                }
            }

            if (!insertDataCommand.DataPattern.ChildGraphPatterns.SelectMany(graph => graph.TriplePatterns).Union(insertDataCommand.DataPattern.TriplePatterns).Any())
            {
                toBeRemoved.Add(insertDataCommand);
            }
        }
    }
}