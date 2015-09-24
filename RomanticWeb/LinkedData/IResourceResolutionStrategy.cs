using RomanticWeb.Entities;

namespace RomanticWeb.LinkedData
{
    /// <summary>Defines an interface of the resource resoulition strategy.</summary>
    public interface IResourceResolutionStrategy
    {
        /// <summary>Resolves a given entity identifier into an entity.</summary>
        /// <param name="id">Entity identifier to be resolved.</param>
        /// <returns>Instance of the <see cref="IEntity" /> describing given <paramref name="id" />.</returns>
        IEntity Resolve(EntityId id);

        /// <summary>Resolves a given entity identifier into a typed entity.</summary>
        /// <typeparam name="T">Type of entity to be resolved.</typeparam>
        /// <param name="id">Entity identifier to be resolved.</param>
        /// <returns>Instance of the <typeparamref name="T"/> describing given <paramref name="id" />.</returns>
        T Resolve<T>(EntityId id) where T : class, IEntity;
    }
}