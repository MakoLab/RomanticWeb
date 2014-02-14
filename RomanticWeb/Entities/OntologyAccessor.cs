using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Entities
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
        private static IDictionary<ProcessingOperation,IResultProcessingStrategy> _resultAggregations;
        private readonly IEntityStore _tripleStore;
        private readonly Entity _entity;
        private readonly Ontology _ontology;
        private readonly INodeConverter _nodeConverter;

        /// <summary>
        /// Creates a new instance of <see cref="OntologyAccessor"/>
        /// </summary>
        internal OntologyAccessor(IEntityStore tripleStore, Entity entity, Ontology ontology, INodeConverter nodeConverter)
        {
            _tripleStore = tripleStore;
            _entity = entity;
            _ontology = ontology;
            _nodeConverter = nodeConverter;
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

        internal IDictionary<ProcessingOperation,IResultProcessingStrategy> ResultAggregations
        {
            get
            {
                if (_resultAggregations==null)
                {
                    _resultAggregations=new Dictionary<ProcessingOperation,IResultProcessingStrategy>();
                    lock (_resultAggregations)
                    {
                        foreach (IResultProcessingStrategy resultProcessingStrategy in ContainerFactory.GetInstancesImplementing<IResultProcessingStrategy>())
                        {
                            object[] attributes=resultProcessingStrategy.GetType().GetCustomAttributes(typeof(ResultProcessingStrategyAttribute),true);
                            if (attributes.Length>0)
                            {
                                ResultProcessingStrategyAttribute resultProcessingStrategyAttribute=(ResultProcessingStrategyAttribute)attributes[0];
                                _resultAggregations[resultProcessingStrategyAttribute.Operation]=resultProcessingStrategy;
                            }
                        }
                    }
                }

                return _resultAggregations;
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
            return true;
        }

        internal Property GetProperty(string binderName)
        {
            var spec=new DynamicPropertyAggregate(binderName);
            return (from prop in Ontology.Properties
                    where prop.PropertyName == spec.Name
                    select prop).SingleOrDefault();
        }

        internal object GetObjects(EntityId entityId, Property property, DynamicPropertyAggregate aggregate)
        {
            LogTo.Trace("Reading property {0}", property.Uri);
            var objectValues=_tripleStore.GetObjectsForPredicate(entityId,property.Uri,null);
            var objects=_nodeConverter.ConvertNodes(objectValues);
            var aggregation=(ResultAggregations.ContainsKey(aggregate.Aggregation)?ResultAggregations[aggregate.Aggregation]:null);
            if (aggregation!=null)
            {
                LogTo.Trace("Performing operation {0} on result nodes", aggregate.Aggregation);
                return aggregation.Process(objects);
            }

            return objects.ToList();
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