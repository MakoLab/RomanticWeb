using ImpromptuInterface.Dynamic;

namespace RomanticWeb
{
    /// <summary>
    /// An RDF entity, which can be used to dynamically access RDF triples
    /// </summary>
    public class Entity : ImpromptuDictionary
    {
        private readonly EntityId _entityId;

        /// <summary>
        /// Creates a new instance of <see cref="Entity"/>
        /// </summary>
        /// <remarks>It will not be backed by <b>any</b> triples, when not created via factory</remarks>
        public Entity(EntityId entityId)
        {
            _entityId = entityId;
        }

        /// <summary>
        /// Gets the entity's identifier
        /// </summary>
        protected internal EntityId Id
        {
            get { return _entityId; }
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            bool gettingMemberSucceeded = base.TryGetMember(binder, out result);
            
            if (!gettingMemberSucceeded)
            {
                throw new UnknownNamespaceException(binder.Name);
            }

            return true;
        }
    }
}
