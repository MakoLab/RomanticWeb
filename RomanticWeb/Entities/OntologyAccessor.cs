using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Entities
{
	/// <summary>
	/// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
	/// </summary>
	/// todo: make a DynamicObject
	[NullGuard(ValidationFlags.OutValues)]
    public sealed class OntologyAccessor:ImpromptuDictionary
	{
		private readonly IEntityStore _tripleSource;
		private readonly Entity _entity;
		private readonly Ontology _ontology;
		private readonly INodeConverter _nodeConverter;

	    /// <summary>
	    /// Creates a new instance of <see cref="OntologyAccessor"/>
	    /// </summary>
	    internal OntologyAccessor(IEntityStore tripleSource, Entity entity, Ontology ontology, INodeConverter nodeConverter)
		{
			_tripleSource = tripleSource;
			_entity = entity;
			_ontology = ontology;
			_nodeConverter = nodeConverter;
	        ResultAggregations=new Lazy<IResultAggregationStrategy,IResultAggregationStrategyMetadata>[0];
		}

        [ImportMany(typeof(IResultAggregationStrategy))]
        public IEnumerable<Lazy<IResultAggregationStrategy, IResultAggregationStrategyMetadata>> ResultAggregations { get; private set; }

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
	        var objectValues=_tripleSource.GetObjectsForPredicate(entityId,property.Uri);
	        var objects=_nodeConverter.ConvertNodes(property.Uri,objectValues);

	        var aggregation=(from agg in ResultAggregations
                             where agg.Metadata.Operation == aggregate.Aggregation
                             select agg).SingleOrDefault();

            if (aggregation!=null)
                {
                return aggregation.Value.Aggregate(objects);
            }

	        return objects.ToList();
	    }
	}
}