using System.Collections;
using System.Dynamic;
using System.Linq;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    [NullGuard(ValidationFlags.OutValues)]
    public class EntityProxy:DynamicObject,IEntity
    {
        private readonly IEntityStore _store;
        private readonly Entity _entity;
        private readonly IEntityMapping _entityMappings;
        private readonly INodeProcessor _processor;

        internal EntityProxy(IEntityStore store, Entity entity, IEntityMapping entityMappings, INodeProcessor processor)
        {
            _store = store;
            _entity = entity;
            _entityMappings = entityMappings;
            _processor = processor;
        }

        public EntityId Id
        {
            get
            {
                return _entity.Id;
            }
        }

        public dynamic this[string member]
        {
            get
            {
                return _entity[member];
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _entity.EnsureIsInitialized();

            var property = _entityMappings.PropertyFor(binder.Name);

            var objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri);
            IList objectsForPredicate = _processor.ProcessNodes(property.Uri,objects).ToList();

            if ((objectsForPredicate.Count>1||property.IsCollection)&&objectsForPredicate.Cast<object>().All(o=>!(o is IList)))
            {
                result=objectsForPredicate;
            }
            else if (objectsForPredicate.Count==0)
            {
                result=null;
            }
            else
            {
                result=objectsForPredicate[0];
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            var entity = obj as IEntity;
            if (entity == null) { return false; }

            return entity.Equals(_entity);
        }

        public override int GetHashCode()
        {
            return this._entity.GetHashCode();
        }

        public override string ToString()
        {
            return _entity.ToString();
        }

        public TInterface AsEntity<TInterface>() where TInterface : class,IEntity
        {
            return _entity.AsEntity<TInterface>();
        }

        public dynamic AsDynamic()
        {
            return _entity;
        }
    }
}