using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;

namespace RomanticWeb.Entities
{
	/// <summary>
	/// An RDF entity, which can be used to dynamically access RDF triples
	/// </summary>
	[NullGuard(ValidationFlags.OutValues)]
	[DebuggerDisplay("Entity <{Id}>")]
	public class Entity:ImpromptuDictionary,IEntity
	{
		private readonly EntityContext _entityContext;
		private readonly EntityId _entityId;
		private readonly IDictionary<Type,object> _knownActLike=new Dictionary<Type,object>();

		private readonly dynamic _asDynamic;

		/// <summary>Creates a new instance of <see cref="Entity"/></summary>
		/// <remarks>It will not be backed by <b>any</b> triples, when not created via factory</remarks>
		public Entity(EntityId entityId)
		{
			this._asDynamic=(dynamic)this;
			this._entityId=entityId;
		}

		internal Entity(EntityId entityId,EntityContext entityContext):this(entityId)
		{
			this._entityContext=entityContext;
		}

        public EntityId Id { get { return this._entityId; } }

        public dynamic AsDynamic() { return this._asDynamic; }

		public override bool TryGetMember(GetMemberBinder binder,out object result)
		{
			// first look for ontology prefix
			bool gettingMemberSucceeded=base.TryGetMember(binder,out result);

			if (gettingMemberSucceeded)
			{
			    return true;
			}

			// then look for properties in ontologies
			if (this.TryGetPropertyFromOntologies(binder,out result))
			{
			    return true;
			}

			return false;
		}

		public TInterface AsEntity<TInterface>() where TInterface : class, IEntity
		{
			if (this._entityContext!=null)
			{
				return this._entityContext.EntityAs<TInterface>(this);
			}

			if (!this._knownActLike.ContainsKey(typeof(TInterface)))
			{
				this._knownActLike[typeof(TInterface)] = new ImpromptuDictionary().ActLike<TInterface>();
			}

			return (TInterface)this._knownActLike[typeof(TInterface)];
		}

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var entity=obj as IEntity;
            if (entity == null) { return false; }
            return this.Id.Equals(entity.Id);
        }

		private bool TryGetPropertyFromOntologies(GetMemberBinder binder,out object result)
		{
		    var matchingPredicates=(from accessor in this.Values.OfType<OntologyAccessor>()
		                            from property in accessor.KnownProperties
		                            where property.PropertyName==binder.Name
		                            select new { accessor,property }).ToList();

			if (matchingPredicates.Count==1)
			{
				var singleMatch=matchingPredicates.Single();
				result=((IObjectAccessor)singleMatch.accessor).GetObjects(this.Id,singleMatch.property);
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
