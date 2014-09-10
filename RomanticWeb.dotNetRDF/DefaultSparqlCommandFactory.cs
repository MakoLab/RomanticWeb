using System;
using System.Collections.Generic;
using System.Linq;
using Resourcer;
using RomanticWeb.Model;
using RomanticWeb.Updates;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using VDS.RDF.Writing.Formatting;
using Triple = VDS.RDF.Triple;

namespace RomanticWeb.DotNetRDF
{
    public class DefaultSparqlCommandFactory : ISparqlCommandFactory
    {
        private static readonly string DeleteEntityGraphCommandText = Resource.AsString("Queries.DeleteEntityGraph.ru");
        private static readonly string ModifyEntityCommandText = Resource.AsString("Queries.ModifyEntityGraph.ru");
        private static readonly string ReconstructCommandText = Resource.AsString("Queries.ReconstructGraph.ru");

        private readonly Dictionary<Type, Func<dynamic, IEnumerable<SparqlUpdateCommand>>> _commandFactories;
        private readonly Uri _metaGraphUri;
        private readonly SparqlUpdateParser _parser = new SparqlUpdateParser();

        public DefaultSparqlCommandFactory(Uri metaGraphUri)
        {
            _metaGraphUri = metaGraphUri;
            _commandFactories = new Dictionary<Type, Func<dynamic, IEnumerable<SparqlUpdateCommand>>>();
            _commandFactories[typeof(GraphDelete)] = delete => CreateGraphDeleteCommand(delete);
            _commandFactories[typeof(GraphUpdate)] = update => CreateGraphUpdateCommand(update);
            _commandFactories[typeof(GraphReconstruct)] = reconstruct => CreateReconstructCommand(reconstruct);
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

        private IEnumerable<SparqlUpdateCommand> CreateGraphDeleteCommand(GraphDelete change)
        {
            var deleteCommands = new SparqlParameterizedString(DeleteEntityGraphCommandText);
            deleteCommands.SetUri("graph", change.Graph.Uri);
            deleteCommands.SetUri("metaGraph", MetaGraphUri);
            deleteCommands.SetUri("entity", change.ChangedEntity.Uri);

            return GetParsedCommands(deleteCommands);
        }

        private IEnumerable<SparqlUpdateCommand> CreateGraphUpdateCommand(GraphUpdate change)
        {
            INodeFactory factory = new NodeFactory();
            var removedTriples = ConvertTriples(change.RemovedQuads, factory);
            var addedTriples = ConvertTriples(change.AddedQuads, factory);

            var commandText = string.Format(ModifyEntityCommandText, removedTriples, addedTriples);
            var deleteCommands = new SparqlParameterizedString(commandText);
            deleteCommands.SetUri("graph", change.Graph.Uri);
            deleteCommands.SetUri("metaGraph", MetaGraphUri);
            deleteCommands.SetUri("entity", change.ChangedEntity.Uri);

            return GetParsedCommands(deleteCommands);
        }

        private IEnumerable<SparqlUpdateCommand> CreateReconstructCommand(GraphReconstruct change)
        {
            INodeFactory factory = new NodeFactory();
            var addedTriples = ConvertTriples(change.AddedQuads, factory);

            var commandText = string.Format(ReconstructCommandText, addedTriples);
            var deleteCommands = new SparqlParameterizedString(commandText);
            deleteCommands.SetUri("graph", change.Graph.Uri);
            deleteCommands.SetUri("metaGraph", MetaGraphUri);

            return GetParsedCommands(deleteCommands);
        }

        private IEnumerable<SparqlUpdateCommand> GetParsedCommands(SparqlParameterizedString commandString)
        {
            return _parser.ParseFromString(commandString).Commands;
        }
    }
}