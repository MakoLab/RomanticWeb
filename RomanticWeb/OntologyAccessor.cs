using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>
	/// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
	/// </summary>
	/// todo: make a DynamicObject
	[NullGuard(ValidationFlags.OutValues)]
	internal sealed class OntologyAccessor : ImpromptuDictionary
	{
		private readonly IEntityStore _tripleSource;
		private readonly Entity _entity;
		private readonly Ontology _ontology;
		private readonly INodeProcessor _nodeProcessor;
	    private readonly Dictionary<AggregateOperation,Func<IEnumerable<object>,object>> _selectorFunctions;

	    /// <summary>
	    /// Creates a new instance of <see cref="OntologyAccessor"/>
	    /// </summary>
	    internal OntologyAccessor(IEntityStore tripleSource, Entity entity, Ontology ontology, INodeProcessor nodeProcessor)
		{
			_tripleSource = tripleSource;
			_entity = entity;
			_ontology = ontology;
			_nodeProcessor = nodeProcessor;

            // todo: extract as MEF exports for extension?
            _selectorFunctions = new Dictionary<AggregateOperation, Func<IEnumerable<object>, object>>
                                   {
                                       { AggregateOperation.Single, Enumerable.Single },
                                       { AggregateOperation.SingleOrDefault, Enumerable.SingleOrDefault },
                                       { AggregateOperation.First, Enumerable.First },
                                       { AggregateOperation.FirstOrDefault, Enumerable.FirstOrDefault },
                                       { AggregateOperation.Has, e=> e.Any() },
                                       { AggregateOperation.Flatten, FlattenResults }
                                   };
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

		    var property=_ontology.Properties.SingleOrDefault(p => p.PropertyName==propertySpec.Name);

		    if (property==null)
		    {
		        property=new Property(propertySpec.Name).InOntology(_ontology);
		    }

		    result=GetObjects(_entity.Id,property,propertySpec);
		    return true;
		}

        internal Property GetProperty(string binderName)
        {
            var spec=new DynamicPropertyAggregate(binderName);
            return (from prop in _ontology.Properties
                    where prop.PropertyName == spec.Name
                    select prop).SingleOrDefault();
        }

	    internal object GetObjects(EntityId entityId, Property property, DynamicPropertyAggregate aggregate)
	    {
	        var objectValues=_tripleSource.GetObjectsForPredicate(entityId,property.Uri);
	        var objects=_nodeProcessor.ProcessNodes(property.Uri,objectValues);

            if (_selectorFunctions.ContainsKey(aggregate.Aggregation))
            {
                var aggregated=_selectorFunctions[aggregate.Aggregation](objects);
                if (aggregated is IEnumerable<object>)
                {
                    return ((IEnumerable<object>)aggregated).ToList();
                }

                return aggregated;
            }

	        return objects.ToList();
	    }

        private IEnumerable<object> FlattenResults(IEnumerable<object> arg)
        {
            foreach (var o in arg)
            {
                if (o is IEnumerable<object>)
                {
                    foreach (var result in FlattenResults((IEnumerable<object>)o))
                    {
                        yield return result;
                    }
                }
                else
                {
                    yield return o;
                }
            }
        }
	}
}