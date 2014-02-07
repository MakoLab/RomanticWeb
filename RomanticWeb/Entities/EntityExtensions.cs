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
        public static TInterface AsEntity<TInterface>(this IEntity entity) where TInterface : class,IEntity
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
                result=Impromptu.ActLike<TInterface>(entity.AsDynamic());
            }

            return result;
        }

        /// <summary>Gets an enumeration containing all RDF types behind given entity.</summary>
        /// <param name="entity">Entity to operate on.</param>
        /// <returns>Returns an enumeration of RDF types for given entity.</returns>
        public static IEnumerable<EntityId> GetTypes(this IEntity entity)
        {
            return (entity!=null?entity.AsEntity<ITypedEntity>().Types.Union(new EntityId[] { new EntityId(RomanticWeb.Vocabularies.Owl.Thing) }):new EntityId[0]);
        }

        /// <summary>Determines if a given entity is of any of the types provided.</summary>
        /// <param name="entity">Entity to operate on.</param>
        /// <param name="types">Enumeration of types to check against.</param>
        /// <returns><b>true</b> if an entity is of any of the given types; othewise <b>false</b>.</returns>
        public static bool Is(this IEntity entity, IEnumerable<EntityId> types)
        {
            return ((entity!=null)&&(types!=null)?entity.GetTypes().Join(types,item => item,item => item,(left,right) => left).Any():false);
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

        /// <summary>Gets the entity context associated with that entity.</summary>
        /// <param name="entity">Source entity</param>
        /// <returns><see cref="IEntityContext" /> instance or null.</returns>
        public static IEntityContext GetContext(this IEntity entity)
        {
            if (entity!=null)
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

            return null;
        }

        /// <summary>
        /// Creates a blank identifier, which will be associated with this entity.
        /// </summary>
        /// <param name="entity">The root entity.</param>
        public static BlankId CreateBlankId(this IEntity entity)
        {
            var blankIdGenerator=entity.GetContext().BlankIdGenerator;
            return new BlankId(blankIdGenerator.Generate(),entity.Id);
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