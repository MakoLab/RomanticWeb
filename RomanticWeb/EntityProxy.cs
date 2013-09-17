using System.Collections;
using System.Dynamic;
using System.Linq;

using RomanticWeb.Mapping.Model;

namespace RomanticWeb
{
    internal class EntityProxy:DynamicObject,IEntity
    {
        private readonly TripleSourceFactoryBase _sourceFactory;
        private readonly Entity _entity;
        private readonly IMapping _mappings;
        private readonly RdfNodeConverter _converter;

        public EntityProxy(TripleSourceFactoryBase source, Entity entity, IMapping mappings, RdfNodeConverter converter)
        {
            _sourceFactory = source;
            _entity = entity;
            _mappings = mappings;
            _converter = converter;
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
            var property = _mappings.PropertyFor(binder.Name);

            ITripleSource source = _sourceFactory.CreateTripleSourceForProperty(_entity.Id, _mappings.PropertyFor(binder.Name));
            IList objectsForPredicate = _converter.Convert(source.GetObjectsForPredicate(_entity.Id, property.Uri), source).ToList();

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