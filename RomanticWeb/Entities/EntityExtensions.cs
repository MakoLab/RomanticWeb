using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            entity = UnwrapProxy(entity);

            if (entity is Entity)
            {
                result=((Entity)entity).AsEntity<TInterface>();
            }
            else if (entity is EntityProxy)
            {
                result=((EntityProxy)entity).AsEntity<TInterface>();
            }

            return result;
        }

        /// <summary>Gets an enumeration containing all RDF types behind given entity.</summary>
        /// <param name="entity">Entity to operate on.</param>
        /// <returns>Returns an enumeration of RDF types for given entity.</returns>
        public static IEnumerable<EntityId> GetTypes(this IEntity entity)
        {
            IEnumerable result=(IEnumerable)entity.AsDynamic().rdf.type;
            return (result!=null?result.Cast<Entity>().Select(item => item.Id).ToArray():new EntityId[0]);
        }

        public static void ForceInitialize(this IEntity entity)
        {
            entity = UnwrapProxy(entity);

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
            entity = UnwrapProxy(entity);

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
            if (entity is IActLikeProxy)
            {
                return ((IActLikeProxy)entity).Original;
            }

            return entity;
        }
    }
}