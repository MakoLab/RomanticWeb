using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Dynamic
{
    /// <summary>
    /// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
    /// </summary>
    /// todo: make a DynamicObject
    [DebuggerDisplay("Ontology Accessor")]
    [NullGuard(ValidationFlags.OutValues)]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    public sealed class OntologyAccessor : ImpromptuDictionary
    {
        private readonly IEntityStore _tripleStore;
        private readonly IEntityContext _context;
        private readonly Entity _entity;
        private readonly Ontology _ontology;
        private readonly IResultTransformerCatalog _resultTransformers;
        private readonly INodeConverter _nodeConverter = new FallbackNodeConverter();

        /// <summary>Creates a new instance of <see cref="OntologyAccessor"/>.</summary>
        internal OntologyAccessor(Entity entity, Ontology ontology, IResultTransformerCatalog resultTransformers)
        {
            _tripleStore = entity.Context.Store;
            _entity = entity;
            _ontology = ontology;
            _resultTransformers = resultTransformers;
            _context = entity.Context;
        }

        /// <summary>Gets the underlying <see cref="Ontologies.Ontology"/>.</summary>
        public Ontology Ontology { get { return _ontology; } }

        /// <summary>Tries to retrieve subjects from the backing RDF source for a dynamically resolved property.</summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _entity.EnsureIsInitialized();

            var propertySpec = new DynamicPropertyAggregate(binder.Name);
            if (!propertySpec.IsValid)
            {
                result = null;
                return false;
            }

            var property = Ontology.Properties.SingleOrDefault(p => p.PropertyName == propertySpec.Name);
            if (property == null)
            {
                property = new Property(propertySpec.Name).InOntology(Ontology);
            }

            result = GetObjects(_entity.Id, property, propertySpec);

            if (propertySpec.IsList)
            {
                var graphOverride = new UnionGraphSelector();
                var head = (IRdfListNode<object>)typeof(EntityExtensions)
                    .GetMethod("AsEntity")
                    .MakeGenericMethod(typeof(IRdfListNode<object>))
                    .Invoke(null, new[] { result });
                ((IEntityProxy)head.UnwrapProxy()).OverrideGraphSelection(graphOverride);
                var rdfListAdapter = new RdfListAdapter<IRdfListOwner, IRdfListNode<object>, object>(_entity.Context, _entity, head, graphOverride);
                result = new ReadOnlyCollection<dynamic>(rdfListAdapter);
            }

            return true;
        }

        internal Property GetProperty(string binderName)
        {
            var spec = new DynamicPropertyAggregate(binderName);
            return (from prop in Ontology.Properties
                    where prop.PropertyName == spec.Name
                    select prop).SingleOrDefault();
        }

        internal object GetObjects(EntityId entityId, Property property, DynamicPropertyAggregate aggregate)
        {
            LogTo.Trace("Reading property {0}", property.Uri);
            var objectValues = _tripleStore.GetObjectsForPredicate(entityId, property.Uri, null);
            var objects = objectValues.Select(ConvertObject);
            var aggregator = _resultTransformers.GetAggregator(aggregate.Aggregation);
            LogTo.Trace("Performing operation {0} on result nodes", aggregate.Aggregation);
            return aggregator.Aggregate(objects);
        }

        private object ConvertObject(Node node)
        {
            var convertObject = _nodeConverter.Convert(node, _context);

            if (convertObject is IEntity)
            {
                return (convertObject as IEntity).AsDynamic();
            }

            return convertObject;
        }

        public class DynamicListNode : ListEntryMap<IRdfListNode<object>, object, FallbackNodeConverter> { }

        private class DebuggerViewProxy
        {
            private readonly OntologyAccessor _accessor;

            public DebuggerViewProxy(OntologyAccessor accessor)
            {
                _accessor = accessor;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Ontology Ontology
            {
                get
                {
                    return _accessor.Ontology;
                }
            }

            public IEntityStore EntityStore
            {
                get
                {
                    return _accessor._tripleStore;
                }
            }
        }
    }
}