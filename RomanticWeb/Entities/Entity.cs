using System;
using System.Collections.Concurrent;
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
	public class Entity:DynamicObject,IEntity
	{
		private readonly EntityContext _entityContext;

	    private readonly EntityId _entityId;
		private readonly IDictionary<Type,object> _knownActLike=new Dictionary<Type,object>();

		private readonly dynamic _asDynamic;

        private readonly IDictionary<string, OntologyAccessor> _ontologyAccessors;

        private bool _isInitialized;

	    /// <summary>Creates a new instance of <see cref="Entity"/></summary>
		/// <remarks>It will not be backed by <b>any</b> triples, when not created via factory</remarks>
		public Entity(EntityId entityId)
		{
			_asDynamic=this;
			_entityId=entityId;
            _ontologyAccessors=new ConcurrentDictionary<string,OntologyAccessor>();
		}

		internal Entity(EntityId entityId,EntityContext entityContext):this(entityId)
		{
		    _entityContext=entityContext;
		}

	    public EntityId Id { get { return _entityId; } }

	    private bool IsInitialized
	    {
	        get
	        {
	            return (_entityId is BlankId)||_isInitialized;
	        }
	    }

	    public object this[string member]
	    {
	        get
	        {
	            return _ontologyAccessors[member];
	        }

            internal set
            {
                var accessor=value as OntologyAccessor;
                if (accessor!=null)
                {
                    _ontologyAccessors.Add(member,accessor);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "Must be OntologyAccessor");
                }
            }
	    }

#pragma warning disable 1591
	    public static bool operator ==(Entity left, IEntity right)
        {
            return Equals(left,right);
        }

        public static bool operator !=(Entity left, IEntity right)
        {
            return !(left == right);
        }
#pragma warning restore 1591

        public dynamic AsDynamic() { return _asDynamic; }

		public override bool TryGetMember(GetMemberBinder binder,out object result)
		{
		    EnsureIsInitialized();

            // first look for ontology prefix
			bool gettingMemberSucceeded=TryGetOntologyAccessor(binder,out result);

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
			if (_entityContext!=null)
			{
				return _entityContext.EntityAs<TInterface>(this);
			}

			if (!_knownActLike.ContainsKey(typeof(TInterface)))
			{
				_knownActLike[typeof(TInterface)] = new ImpromptuDictionary().ActLike<TInterface>();
			}

			return (TInterface)_knownActLike[typeof(TInterface)];
		}

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var entity=obj as IEntity;
            if (entity == null) { return false; }
            return Id.Equals(entity.Id);
        }

        internal void EnsureIsInitialized()
        {
            if ((_entityContext != null)&&!IsInitialized)
            {
                _entityContext.InitializeEnitity(this);
                _isInitialized=true;
            }
        }

        private bool TryGetOntologyAccessor(GetMemberBinder binder, out object result)
        {
            if (_ontologyAccessors.ContainsKey(binder.Name))
            {
                result = _ontologyAccessors[binder.Name];
                return true;
            }

            result = null;
            return false;
        }

		private bool TryGetPropertyFromOntologies(GetMemberBinder binder,out object result)
		{
		    var matchingPredicates=(from accessor in _ontologyAccessors.Values
		                            from property in accessor.KnownProperties
		                            where property.PropertyName==binder.Name
		                            select new { accessor,property }).ToList();

			if (matchingPredicates.Count==1)
			{
				var singleMatch=matchingPredicates.Single();
				result=singleMatch.accessor.GetObjects(Id,singleMatch.property);
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
