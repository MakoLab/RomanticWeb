using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace RomanticWeb
{
    public class Entity : ImpromptuDictionary
    {
        private readonly EntityId _entityId;

        public Entity(EntityId entityId)
        {
            _entityId = entityId;
        }

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
