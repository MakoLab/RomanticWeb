using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Resourcer;
using RomanticWeb.Entities;

using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    internal class DatasetChangesCommandSetConverter
    {
        private static readonly string DeleteEntityGraphCommandText = Resource.AsString("Queries.DeleteEntityGraph.ru");
        private static readonly string DeleteDataCommandText = Resource.AsString("Queries.DeleteData.ru");
        private static readonly string InsertEntityCommandText = Resource.AsString("Queries.InsertEntity.ru");
        private static readonly string InsertBlankNodeCommandText = Resource.AsString("Queries.InsertBlankNode.ru");

        private readonly IEnumerable<Triple> _currentTriples;
        private readonly Uri _metaGraphUri;
        private readonly DatasetChanges _datasetChanges;
        private IGraph _nodeFactory;
        private SparqlUpdateParser _parser = null;

        internal DatasetChangesCommandSetConverter(IEnumerable<Triple> currentTriples, Uri metaGraphUri, DatasetChanges datasetChanges)
        {
            _currentTriples = currentTriples;
            _metaGraphUri = metaGraphUri;
            _datasetChanges = datasetChanges;
        }

        internal SparqlUpdateCommandSet ConvertToSparqlUpdateCommandSet()
        {
            _nodeFactory = new Graph();
            _parser = new SparqlUpdateParser();
            return new SparqlUpdateCommandSet(
                GetDeleteEntityCommands(_datasetChanges.EntitiesRemoved).Concat(
                GetDeleteDataCommands(_datasetChanges.QuadsRemoved)).Concat(
                GetInsertDataCommands(_datasetChanges.QuadsAdded, false)).Concat(
                GetInsertDataCommands(_datasetChanges.EntitiesReconstructed)));
        }

        private IEnumerable<SparqlUpdateCommand> GetDeleteEntityCommands(IEnumerable<EntityId> entityGraphs)
        {
            foreach (EntityId entityId in entityGraphs)
            {
                SparqlParameterizedString delete = new SparqlParameterizedString(DeleteEntityGraphCommandText);
                delete.SetUri("metaGraph", _metaGraphUri);
                delete.SetUri("entityId", entityId.Uri);
                foreach (var command in _parser.ParseFromString(delete).Commands)
                {
                    yield return command;
                }
            }
        }

        private IEnumerable<SparqlUpdateCommand> GetDeleteDataCommands(IEnumerable<RomanticWeb.Model.EntityQuad> quads)
        {
            foreach (var quad in quads)
            {
                SparqlParameterizedString insert = new SparqlParameterizedString(DeleteDataCommandText);
                insert.SetParameter("subject", quad.Subject.UnWrapNode(_nodeFactory));
                insert.SetParameter("predicate", quad.Predicate.UnWrapNode(_nodeFactory));
                insert.SetParameter("object", quad.Object.UnWrapNode(_nodeFactory));
                insert.SetUri("graph", quad.Graph.UnWrapGraphUri());

                foreach (var command in _parser.ParseFromString(insert).Commands)
                {
                    yield return command;
                }
            }
        }

        private IEnumerable<SparqlUpdateCommand> GetInsertDataCommands(IEnumerable<RomanticWeb.Model.EntityQuad> quads, bool withDelete = true)
        {
            Uri lastGraphUri = null;
            EntityId lastEntityId = null;
            StringBuilder entityPatterns = new StringBuilder(1024);
            IDictionary<string, INode> variables = new Dictionary<string, INode>();
            int index = 0;
            foreach (var quad in quads)
            {
                Uri entityGraphUri = quad.Graph.UnWrapGraphUri();
                if ((entityGraphUri != lastGraphUri) || (quad.EntityId != lastEntityId))
                {
                    if ((lastGraphUri != null) && (lastEntityId != null))
                    {
                        foreach (var command in GetInsertCommands(lastGraphUri, lastEntityId, entityPatterns.ToString(), variables, (lastEntityId is BlankId ? false : withDelete)))
                        {
                            yield return command;
                        }
                    }

                    entityPatterns.Clear();
                    variables.Clear();
                    lastGraphUri = entityGraphUri;
                    lastEntityId = quad.EntityId;
                }

                entityPatterns.AppendFormat("@subject{0} @predicate{0} @object{0} . ", index);
                variables["subject" + index.ToString()] = quad.Subject.UnWrapNode(_nodeFactory);
                variables["predicate" + index.ToString()] = quad.Predicate.UnWrapNode(_nodeFactory);
                variables["object" + index.ToString()] = quad.Object.UnWrapNode(_nodeFactory);
                index++;
            }

            if ((lastGraphUri != null) && (lastEntityId != null))
            {
                foreach (var command in GetInsertCommands(lastGraphUri, lastEntityId, entityPatterns.ToString(), variables, (lastEntityId is BlankId ? false : withDelete)))
                {
                    yield return command;
                }
            }
        }

        private IEnumerable<SparqlUpdateCommand> GetInsertCommands(Uri entityGraphUri, EntityId entityId, string entityPatterns, IDictionary<string, INode> variables, bool withDelete = true)
        {
            SparqlParameterizedString modify = new SparqlParameterizedString(
                (withDelete ? DeleteEntityGraphCommandText : System.String.Empty) +
                (entityId is BlankId ? InsertBlankNodeCommandText : InsertEntityCommandText).Replace("@subject @predicate @object . ", entityPatterns));
            modify.SetUri("metaGraph", _metaGraphUri);
            modify.SetUri("graph", entityGraphUri);
            if (!(entityId is BlankId))
            {
                modify.SetUri("entityId", entityId.Uri);
            }

            foreach (var variable in variables)
            {
                modify.SetParameter(variable.Key, variable.Value);
            }

            foreach (var command in _parser.ParseFromString(modify).Commands)
            {
                yield return command;
            }
        }
    }
}