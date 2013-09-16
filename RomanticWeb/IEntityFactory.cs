using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb
{
	/// <summary>Defines methods for factories, which produce <see cref="Entity"/> instances.</summary>
	public interface IEntityFactory
	{
	    /// <summary>Enables given entity factory to be LINQ queryable with respect to the underlying triple store.</summary>
		/// <returns>A queryable collection of entities.</returns>
		IQueryable<Entity> AsQueryable();

		/// <summary>Enables given entity factory to be LINQ queryable with respect to the underlying triple store.</summary>
		/// <typeparam name="T">Type to be used when returning a typed entity.</typeparam>
		/// <returns>A queryable collection of typed entities.</returns>
		IQueryable<T> AsQueryable<T>() where T:class,IEntity;

		/// <summary>Creates a new entity.</summary>
		/// <param name="entityId">Entity identifier</param>
		/// <returns>Instance of an entity wih given identifier or null.</returns>
		Entity Create(EntityId entityId);

		/// <summary>Creates a new typed entity.</summary>
		/// <typeparam name="T">Type to be used when returning a typed entity.</typeparam>
		/// <param name="entityId">Entity identifier</param>
		/// <returns>Typed instance of an entity wih given identifier or null.</returns>
		T Create<T>(EntityId entityId) where T:class,IEntity;

		/// <summary>Create an enumerable collection of entities beeing a result of a SPARQL construct query.</summary>
		/// <param name="sparqlConstruct">SPARQL construct query.</param>
		/// <returns>An enumerable collection of entities beeing a result of a SPARQL construct query</returns>
		IEnumerable<Entity> Create(string sparqlConstruct);

		/// <summary>Create an enumerable collection of typed entities beeing a result of a SPARQL construct query.</summary>
		/// <typeparam name="T">Type to be used when returning a typed entity.</typeparam>
		/// <param name="sparqlConstruct">SPARQL construct query.</param>
		/// <returns>An enumerable collection of typed entities beeing a result of a SPARQL construct query</returns>
		IEnumerable<T> Create<T>(string sparqlConstruct) where T:class,IEntity;
	}
}