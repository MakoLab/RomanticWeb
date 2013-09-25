using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	/// <summary>Provides an LINQ compatible access to the triple store.</summary>
	/// <typeparam name="T">Type of entities to be queried.</typeparam>
	public class EntityQueryable<T>:QueryableBase<T>
	{
		#region Constructors
		/// <summary>Creates an instance of the queryable entity source.</summary>
		/// <param name="provider">Query provider to be used by this queryable instance.</param>
		/// <param name="expression">Expression to be parsed.</param>
		public EntityQueryable(IQueryProvider provider,Expression expression):base(provider,expression)
		{
			if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
			{
				throw new ArgumentOutOfRangeException("expression");
			}
		}

		/// <summary>Creates an instance of the queryable entity factory.</summary>
		/// <param name="provider">Query provider to be used by this queryable instance.</param>
		protected internal EntityQueryable(IEntityContext entityContext,IMappingsRepository mappings,IOntologyProvider ontologyProvider):base(
			QueryParser.CreateDefault(),new EntityQueryExecutor(entityContext,mappings,ontologyProvider))
		{
			if (!typeof(IEntity).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentOutOfRangeException("T",System.String.Format("Expected '{0}' derived type, but found '{1}'.",typeof(Entity).FullName,typeof(T).FullName));
			}
		}

		/// <summary>Creates an instance of the queryable entity source with contsant expression.</summary>
		/// <param name="provider">Query provider to be used by this queryable instance.</param>
		protected internal EntityQueryable(IQueryProvider provider):base(provider)
		{
		}
		#endregion
	}
}