using System.Collections.Generic;
using ImpromptuInterface;

namespace RomanticWeb.Entities
{
    /// <summary>Provides useful extensions methods for entities.</summary>
    public static class EntityExtensions
    {
        /// <summary>Gets the entity as a dynamic object.</summary>
        /// <param name="entity">Target entity to be converted to dynamic.</param>
        public static dynamic AsDynamic(this IEntity entity)
        {
            dynamic result=null;
            entity=UnwrapProxy(entity);

            if (entity is Entity)
            {
                result=((Entity)entity).AsDynamic();
            }
            else if (entity is EntityProxy)
            {
                result=((EntityProxy)entity).AsDynamic();
            }
            else if (entity is IActLikeProxy)
            {
                result=((IActLikeProxy)entity).Original;
            }
            else
            {
                result=(dynamic)entity;
            }

            return result;
        }

        /// <summary>Wraps the entity as a given statically typed type.</summary>
        public static TInterface AsEntity<TInterface>(this IEntity entity) where TInterface:class,IEntity
        {
            TInterface result=null;
            entity=UnwrapProxy(entity);

            if (entity is Entity)
            {
                result=((Entity)entity).AsEntity<TInterface>();
            }
            else if (entity is EntityProxy)
            {
                result=((EntityProxy)entity).AsEntity<TInterface>();
            }
            else
            {
                result=entity.AsDynamic().ActLike<TInterface>();
            }

            return result;
        }

        /// <summary>Gets an enumeration containing all RDF types behind given entity.</summary>
        /// <param name="entity">Entity to operate on.</param>
        /// <returns>Returns an enumeration of RDF types for given entity.</returns>
        public static IEnumerable<EntityId> GetTypes(this IEntity entity)
        {
            return entity.AsEntity<ITypedEntity>().Types;
        }

        /// <summary>
        /// Forces lazy initialization of <paramref name="entity"/>
        /// </summary>
        public static void ForceInitialize(this IEntity entity)
        {
            entity=UnwrapProxy(entity);

            if (entity is Entity)
            {
                ((Entity)entity).EnsureIsInitialized();
            }
            else if (entity is EntityProxy)
            {
                ((EntityProxy)entity).AsDynamic().EnsureIsInitialized();
            }
        }

        // todo: maybe this should be reconsidered
        public static IEntityContext GetContext(this IEntity entity)
        {
            entity=UnwrapProxy(entity);

            if (entity is Entity)
            {
                return ((Entity)entity).EntityContext;
            }

            if (entity is EntityProxy)
            {
                return ((EntityProxy)entity).AsDynamic().EntityContext;
            }

            return ((dynamic)entity).EntityContext;
        }

        private static IEntity UnwrapProxy(IEntity entity)
        {
            if ((entity is IActLikeProxy)&&(((IActLikeProxy)entity).Original is IEntity))
            {
                return ((IActLikeProxy)entity).Original;
            }

            return entity;
        }
    }
}