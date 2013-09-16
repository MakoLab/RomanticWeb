using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;

namespace RomanticWeb
{
	/// <summary>
	/// An RDF entity, which can be used to dynamically access RDF triples
	/// </summary>
	[NullGuard(ValidationFlags.OutValues)]
	[DebuggerDisplay("Entity <{Id}>")]
	public class Entity:ImpromptuDictionary,IEntity
	{
		private readonly EntityFactory _entityFactory;
		private readonly EntityId _entityId;
		private readonly IDictionary<Type,object> _knownActLike=new Dictionary<Type,object>();

		private readonly dynamic _asDynamic;

		/// <summary>Creates a new instance of <see cref="Entity"/></summary>
		/// <remarks>It will not be backed by <b>any</b> triples, when not created via factory</remarks>
		public Entity(EntityId entityId)
		{
			_asDynamic=(dynamic)this;
			_entityId=entityId;
		}

		internal Entity(EntityId entityId,EntityFactory entityFactory):this(entityId)
		{
			_entityFactory=entityFactory;
		}

        public EntityId Id { get { return _entityId; } }

        public dynamic AsDynamic { get { return _asDynamic; } }

		public override bool TryGetMember(GetMemberBinder binder,out object result)
		{
			// first look for ontology prefix
			bool gettingMemberSucceeded=base.TryGetMember(binder,out result);

			if (gettingMemberSucceeded)
			{
			    return true;
			}

			// then look for properties in ontologies
			if (TryGetPropertyFromOntologies(binder,out result))
			{
			    return true;
			}

			return false;
		}

		public TInterface AsEntity<TInterface>() where TInterface : class, IEntity
		{
			if (_entityFactory!=null)
			{
				return _entityFactory.EntityAs<TInterface>(this);
			}

			if (!_knownActLike.ContainsKey(typeof(TInterface)))
			{
				_knownActLike[typeof(TInterface)] = new ImpromptuDictionary().ActLike<TInterface>();
			}

			return (TInterface)_knownActLike[typeof(TInterface)];
		}

		private bool TryGetPropertyFromOntologies(GetMemberBinder binder,out object result)
		{
			var matchingPredicates=(from accessor in Values.OfType<OntologyAccessor>()
							          from property in accessor.KnownProperties
									where property.PropertyName==binder.Name
							          select new
							              {
							                  accessor,
							                  property
							              }).ToList();

			if (matchingPredicates.Count==1)
			{
				var singleMatch=matchingPredicates.Single();
				result=((IObjectAccessor)singleMatch.accessor).GetObjects(Id,singleMatch.property);
				return true;
			}

			if (matchingPredicates.Count==0)
			{
				result=null;
				return false;
			}

			var matchedPropertiesQNames=matchingPredicates.Select(pair => pair.property.Ontology.Prefix);
			throw new AmbiguousPropertyException(binder.Name,matchedPropertiesQNames);
		}
	}
}
