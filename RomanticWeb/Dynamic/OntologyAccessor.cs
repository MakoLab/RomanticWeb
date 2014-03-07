using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
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
    public sealed class OntologyAccessor:ImpromptuDictionary
    {
        private readonly IEntityStore _tripleStore;
        private readonly Entity _entity;
        private readonly Ontology _ontology;
        private readonly INodeConverter _nodeConverter;
        private readonly IResultTransformerCatalog _resultTransformers;
        private readonly IEntityContext _entityContext;

        /// <summary>
        /// Creates a new instance of <see cref="OntologyAccessor"/>
        /// </summary>
        internal OntologyAccessor(Entity entity,Ontology ontology,IResultTransformerCatalog resultTransformers,IConverterCatalog converters)
        {
            _tripleStore=entity.Context.Store;
            _entity=entity;
            _ontology=ontology;
            _entityContext=entity.Context;
            _nodeConverter=new NodeConverter(_entityContext,converters);
            _resultTransformers=resultTransformers;
        }

        /// <summary>
        /// Gets the underlying <see cref="Ontologies.Ontology"/>
        /// </summary>
        public Ontology Ontology
        {
            get
            {
                return _ontology;
            }
        }

        /// <summary>
        /// Tries to retrieve subjects from the backing RDF source for a dynamically resolved property
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _entity.EnsureIsInitialized();

            var propertySpec=new DynamicPropertyAggregate(binder.Name);

            if (!propertySpec.IsValid)
            {
                result=null;
                return false;
            }

            var property=Ontology.Properties.SingleOrDefault(p => p.PropertyName==propertySpec.Name);

            if (property==null)
            {
                property=new Property(propertySpec.Name).InOntology(Ontology);
            }

            result=GetObjects(_entity.Id,property,propertySpec);

            if (propertySpec.IsList)
            {
                var graphOverride=new UnionGraphSelector();
                var head=((IEntity)result).AsEntity<IRdfListNode>();
                ((IEntityProxy)head.UnwrapProxy()).OverrideGraphSelection(graphOverride);
                var rdfListAdapter=new RdfListAdapter<dynamic>(_entity.Context,head,graphOverride);
                result=new ReadOnlyCollection<dynamic>(rdfListAdapter);
            }

            return true;
        }

        internal Property GetProperty(string binderName)
        {
            var spec=new DynamicPropertyAggregate(binderName);
            return (from prop in Ontology.Properties
                    where prop.PropertyName == spec.Name
                    select prop).SingleOrDefault();
        }

        internal object GetObjects(EntityId entityId,Property property,DynamicPropertyAggregate aggregate)
        {
            LogTo.Trace("Reading property {0}",property.Uri);
            var objectValues=_tripleStore.GetObjectsForPredicate(entityId,property.Uri,null);
            var objects=_nodeConverter.ConvertNodes(objectValues);
            var aggregator=_resultTransformers.GetAggregator(aggregate.Aggregation);
            LogTo.Trace("Performing operation {0} on result nodes",aggregate.Aggregation);
            return aggregator.Aggregate(objects);
        }

        private class DebuggerViewProxy
        {
            private readonly OntologyAccessor _accessor;

            public DebuggerViewProxy(OntologyAccessor accessor)
            {
                _accessor=accessor;
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