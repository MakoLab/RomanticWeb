using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using NullGuard;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;
using RomanticWeb.Vocabularies;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace RomanticWeb.LinkedData
{
    /// <summary>Provides a predefined Uri list resource resolution strategy.</summary>
    /// <remarks>This provider is designed to cover resolving named individuals from their respective ontologies beeing in relations with resources in the main entity context.</remarks>
    public class UrlMatchingResourceResolutionStrategy : IResourceResolutionStrategy
    {
        private const string TextTurtle = "text/turtle";
        private const string ApplicationTurtle = "application/turtle";
        private const string ApplicationXTurtle = "application/x-turtle";
        private const string TextNTriplesTurtle = "text/n-triples+turtle";
        private const string ApplicationRdfXml = "application/rdf+xml";
        private const string ApplicationOwlXml = "application/owl+xml";
        private const string ApplicationNTriples = "application/ntriples";
        private const string ApplicationNTriples2 = "application/n-triples";
        private const string ApplicationXnTriples = "application/x-ntriples";
        private const string ApplicationRdfTriples = "application/rdf-triples";
        private const string TextPlain = "text/plain";
        private const string TextN3 = "text/n3";
        private const string TextRdfN3 = "text/rdf+n3";

        private static readonly string Accept = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}; q=0.5,*.*", TextTurtle, ApplicationTurtle, ApplicationXTurtle, TextNTriplesTurtle, ApplicationRdfXml, ApplicationOwlXml, ApplicationNTriples, ApplicationNTriples2, ApplicationXnTriples, ApplicationRdfTriples, TextPlain, TextN3, TextRdfN3);

        private readonly Lazy<IEntityContext> _entityContext;
        private readonly ITripleStore _tripleStore = new ThreadSafeTripleStore();
        private readonly IGraph _metaGraph = new Graph();
        private readonly IUriNode _predicateNode;
        private readonly IEnumerable<Uri> _baseUris;
        private readonly ISet<string> _dereferencedResources = new HashSet<string>();
        private readonly Func<Uri, WebRequest> _webRequestFactory;
        private readonly INamedGraphSelector _namedGraphSelector;

        /// <summary>Initializes a new instance of the <see cref="UrlMatchingResourceResolutionStrategy" /> class.</summary>
        /// <param name="ontology">Optional ontology provider.</param>
        /// <param name="mappingAssemblies">Optional mapping assemblies.</param>
        /// <param name="baseUris">Base uris.</param>
        /// <param name="webRequestFactory">Web request factory method.</param>
        public UrlMatchingResourceResolutionStrategy([AllowNull] IOntologyProvider ontology, [AllowNull] IEnumerable<Assembly> mappingAssemblies, IEnumerable<Uri> baseUris, [AllowNull] Func<Uri, WebRequest> webRequestFactory = null)
        {
            _namedGraphSelector = new BaseUriNamedGraphSelector(_baseUris = baseUris);
            _metaGraph.BaseUri = new Uri("urn:meta:graph");
            _tripleStore.Add(_metaGraph);
            _entityContext = new Lazy<IEntityContext>(() => CreateEntityContext(ontology, mappingAssemblies));
            _webRequestFactory = webRequestFactory ?? CreateRequest;
            _predicateNode = _metaGraph.CreateUriNode(Foaf.primaryTopic);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public IEntity Resolve(EntityId id)
        {
            if ((id is BlankId) || (!id.Uri.Scheme.ToLower().StartsWith("http")))
            {
                throw new ArgumentOutOfRangeException("id");
            }

            if (!_baseUris.Any(uri => id.Uri.AbsoluteUri.StartsWith(uri.AbsoluteUri)))
            {
                return null;
            }

            if (_dereferencedResources.Contains(id.Uri.AbsoluteUri))
            {
                return _entityContext.Value.Load<IEntity>(id);
            }

            Uri dereferencableUri = new Uri(id.Uri.GetLeftPart(UriPartial.Query));
            var graphUri = _namedGraphSelector.SelectGraph(id, null, null);
            var graph = _tripleStore.Graphs.FirstOrDefault(item => AbsoluteUriComparer.Default.Equals(item.BaseUri, graphUri));
            if (graph == null)
            {
                _tripleStore.Add(graph = new Graph() { BaseUri = graphUri });
            }

            var resourceGraph = ObtainResourceData(dereferencableUri);
            AssertGraph(resourceGraph, graph);
            return _entityContext.Value.Load<IEntity>(id);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public T Resolve<T>(EntityId id) where T : class, IEntity
        {
            var result = Resolve(id);
            return (result == null ? null : result.AsEntity<T>());
        }

        private static WebRequest CreateRequest(Uri uri)
        {
            var result = WebRequest.CreateHttp(uri);
            result.Method = WebRequestMethods.Http.Get;
            result.Accept = Accept;
            result.UserAgent = "RomanticWeb";
            return result;
        }

        private static void ProcessTriplesDeserialization(Stream stream, IGraph graph, string accepted)
        {
            IRdfReader reader = null;
            switch (accepted)
            {
                case TextTurtle:
                case ApplicationTurtle:
                case ApplicationXTurtle:
                case TextNTriplesTurtle:
                    reader = new TurtleParser();
                    break;
                case ApplicationOwlXml:
                case ApplicationRdfXml:
                    reader = new RdfXmlParser();
                    break;
                case ApplicationNTriples:
                case ApplicationNTriples2:
                case ApplicationXnTriples:
                case ApplicationRdfTriples:
                case TextPlain:
                    reader = new NTriplesParser();
                    break;
                case TextN3:
                case TextRdfN3:
                    reader = new Notation3Parser();
                    break;
            }

            using (var textWriter = new StreamReader(stream))
            {
                reader.Load(graph, textWriter);
            }
        }

        private IGraph ObtainResourceData(Uri uri)
        {
            var request = _webRequestFactory(uri);
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                var result = new Graph() { BaseUri = uri };
                ProcessTriplesDeserialization(stream, result, response.ContentType);
                return result;
            }
        }

        private void AssertGraph(IGraph resourceGraph, IGraph graph)
        {
            var graphNode = _metaGraph.CreateUriNode(graph.BaseUri);
            foreach (var triple in resourceGraph.Triples)
            {
                if (triple.Subject is IUriNode)
                {
                    var uriNode = (IUriNode)triple.Subject;
                    _metaGraph.Assert(graphNode, _predicateNode, _metaGraph.CreateUriNode(uriNode.Uri));
                    _dereferencedResources.Add(uriNode.Uri.AbsoluteUri);
                }

                graph.Assert(triple);
            }
        }

        private IEntityContext CreateEntityContext([AllowNull] IOntologyProvider ontologyProvider, [AllowNull] IEnumerable<Assembly> mappingAssemblies)
        {
            var factory = new EntityContextFactory()
                .WithMappings(builder => BuildMappingAssemblies(builder, mappingAssemblies))
                .WithDefaultOntologies()
                .WithMetaGraphUri(_metaGraph.BaseUri)
                .WithNamedGraphSelector(_namedGraphSelector)
                .WithDotNetRDF(_tripleStore);
            if (ontologyProvider != null)
            {
                factory = factory.WithOntology(ontologyProvider);
            }

            return factory.CreateContext();
        }

        private void BuildMappingAssemblies(MappingBuilder builder, [AllowNull] IEnumerable<Assembly> mappingAssemblies)
        {
            if (mappingAssemblies == null)
            {
                return;
            }

            foreach (var assembly in mappingAssemblies)
            {
                builder.FromAssembly(assembly);
            }
        }
    }
}