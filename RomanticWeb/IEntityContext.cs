using System;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    /// <summary>Defines methods for factories, which produce <see cref="Entity"/> instances.</summary>
    public interface IEntityContext:IDisposable
    {
        /// <summary>Gets the underlying in-memory store.</summary>
        IEntityStore Store { get; }

        /// <summary>Gets a value indicating whether the underlying store has any changes.</summary>
        bool HasChanges { get; }

        /// <summary>Converts this context into a LINQ queryable data source.</summary>
        /// <returns>A LINQ querable data source.</returns>
        IQueryable<Entity> AsQueryable();

        /// <summary>Converts this context into a LINQ queryable data source of entities of given type.</summary>
        /// <typeparam name="T">Type of entities to work with.</typeparam>
        /// <returns>A LINQ queryable data source of entities of given type.</returns>
        IQueryable<T> AsQueryable<T>() where T:class,IEntity;

        /// <summary>Loads an existing typed entity.</summary>
        /// <typeparam name="T">Type to be used when returning a typed entity.</typeparam>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="checkIfExist"></param>
        /// <returns>Typed instance of an entity wih given identifier or null.</returns>
        T Load<T>(EntityId entityId,bool checkIfExist) where T:class,IEntity;

        /// <summary>Loads an existing typed entity.</summary>
        /// <typeparam name="T">Type to be used when returning a typed entity.</typeparam>
        /// <param name="entityId">Entity identifier</param>
        /// <returns>Typed instance of an entity wih given identifier or null.</returns>
        T Load<T>(EntityId entityId) where T:class,IEntity;

        /// <summary>Creates a new typed entity.</summary>
        /// <typeparam name="T">Type to be used when returning a typed entity.</typeparam>
        /// <param name="entityId">Entity identifier</param>
        T Create<T>(EntityId entityId) where T:class,IEntity;

        /// <summary>Creates a new entity.</summary>
        /// <param name="entityId">Entity identifier</param>
        Entity Create(EntityId entityId);

        /// <summary>Saves all changes to the underlying store.</summary>
        void Commit();
    }
}