using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using NullGuard;
using RomanticWeb.Dynamic;

namespace RomanticWeb.Entities
{
    /// <summary>An RDF entity, which can be used to dynamically access RDF triples.</summary>
    [NullGuard(ValidationFlags.OutValues)]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(DebuggerDisplayProxy))]
    public class Entity : DynamicObject, IEntity
    {
        #region Fields
        private readonly IEntityContext _context;
        private readonly EntityId _entityId;
        private readonly IDictionary<string, OntologyAccessor> _ontologyAccessors;
        private bool _isInitialized;
        #endregion

        #region Constructors
        /// <summary>Creates a new instance of <see cref="Entity"/>.</summary>
        /// <param name="entityId">IRI of the entity.</param>
        /// <remarks>It will not be backed by <b>any</b> triples, when not created via factory.</remarks>
        internal Entity(EntityId entityId)
        {
            if (!entityId.Uri.IsAbsoluteUri)
            {
                throw new ArgumentException("The identifier must be an absolute URI", "entityId");
            }

            _entityId = entityId;
            _ontologyAccessors = new ConcurrentDictionary<string, OntologyAccessor>();
        }

        /// <summary>Creates a new instance of <see cref="Entity"/> with given entity context.</summary>
        /// <param name="entityId">IRI of the entity.</param>
        /// <param name="context">Entity context to be attached to this entity.</param>
        internal Entity(EntityId entityId, IEntityContext context)
            : this(entityId)
        {
            _context = context;
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public EntityId Id { get { return _entityId; } }

        /// <inheritdoc/>
        public IEntityContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>Determines if the entity was initialized.</summary>
        private bool IsInitialized
        {
            get
            {
                return (_entityId is BlankId) || _isInitialized;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                var format = string.Format("Entity <{0}>", Id);

                if (!IsInitialized)
                {
                    format += " (uninitialized)";
                }

                return format;
            }
        }

        /// <summary>Gets or sets ontology based members.</summary>
        /// <param name="member">Ontology based member.</param>
        /// <returns>Ontology based member.</returns>
        public object this[string member]
        {
            get
            {
                return _ontologyAccessors[member];
            }

            internal set
            {
                var accessor = value as OntologyAccessor;
                if (accessor != null)
                {
                    _ontologyAccessors.Add(member, accessor);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "Must be OntologyAccessor");
                }
            }
        }
        #endregion

        #region Operators
        /// <summary>Checks for equality between two entities.</summary>
        /// <param name="left">Left operand of the check.</param>
        /// <param name="right">Right operand of the check.</param>
        /// <returns><b>True</b> if entities has equal IRI's, otherwise <b>false</b>.</returns>
        public static bool operator ==(Entity left, IEntity right)
        {
            return Equals(left, right);
        }

        /// <summary>Checks for inequality between two entities.</summary>
        /// <param name="left">Left operand of the check.</param>
        /// <param name="right">Right operand of the check.</param>
        /// <returns><b>False</b> if entities has equal IRI's, otherwise <b>true</b>.</returns>
        public static bool operator !=(Entity left, IEntity right)
        {
            return !(left == right);
        }
        #endregion

        #region Public methods
        /// <summary>Converts this entity as an dynamic object.</summary>
        /// <returns>Dynamic object beeing same entity.</returns>
        public dynamic AsDynamic() { return this; }

        /// <summary>Tries to resolve a dynamic member.</summary>
        /// <param name="binder">Binder context with details on which member is going to be resolved.</param>
        /// <param name="result">Result of the member resolution.</param>
        /// <returns><b>True</b> if the member was resolved sucessfuly, otherwise <b>false</b>.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            EnsureIsInitialized();
            bool gettingMemberSucceeded = TryGetOntologyAccessor(binder, out result);
            if (gettingMemberSucceeded)
            {
                return true;
            }

            if (TryGetPropertyFromOntologies(binder, out result))
            {
                return true;
            }

            return false;
        }

        /// <summary>Transforms given entity into a strongly typed interface.</summary>
        /// <typeparam name="TInterface">Strongly typed interface to be transformed into.</typeparam>
        /// <returns>Proxy beeing a dynamic implementation of a given interface.</returns>
        public TInterface AsEntity<TInterface>() where TInterface : class, IEntity
        {
            return _context.EntityAs<TInterface>(this);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj"></param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            var entity = obj as IEntity;
            if (entity == null) { return false; }
            return Id.Equals(entity.Id);
        }

#pragma warning disable
        public override string ToString()
        {
            return string.Format("<{0}>", Id);
        }
#pragma warning restore

        #endregion

        #region Non-public methods
        /// <summary>Ensures the entity is initialized and filled with data.</summary>
        internal void EnsureIsInitialized()
        {
            if ((Context != null) && !IsInitialized)
            {
                _context.InitializeEnitity(this);
                _isInitialized = true;
            }
        }

        internal void MarkAsInitialized()
        {
            _isInitialized = true;
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

        private bool TryGetPropertyFromOntologies(GetMemberBinder binder, out object result)
        {
            var matchingPredicates = (from accessor in _ontologyAccessors.Values
                                      let property = accessor.GetProperty(binder.Name)
                                      where property != null
                                      select new { accessor, property }).ToList();

            if (matchingPredicates.Count == 1)
            {
                var singleMatch = matchingPredicates.Single();
                result = singleMatch.accessor.GetObjects(Id, singleMatch.property, new DynamicPropertyAggregate(binder.Name));
                return result != null;
            }

            if (matchingPredicates.Count == 0)
            {
                result = null;
                return false;
            }

            var matchedPropertiesQNames = matchingPredicates.Select(pair => pair.property.Ontology.Prefix);
            throw new AmbiguousPropertyException(binder.Name, matchedPropertiesQNames);
        }
        #endregion

        private class DebuggerDisplayProxy
        {
            private readonly Entity _entity;

            public DebuggerDisplayProxy(Entity entity)
            {
                _entity = entity;
            }

            public EntityId Id
            {
                get
                {
                    return _entity.Id;
                }
            }

            public ICollection<string> KnownOntologies
            {
                get
                {
                    return _entity._ontologyAccessors.Keys;
                }
            }

            public IEntityContext Context
            {
                get
                {
                    return _entity._context;
                }
            }

            public bool IsInitialized
            {
                get
                {
                    return _entity.IsInitialized;
                }
            }
        }
    }
}