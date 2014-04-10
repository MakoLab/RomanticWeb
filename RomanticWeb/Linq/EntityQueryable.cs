using System.Linq;
using System.Linq.Expressions;
using NullGuard;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;

namespace RomanticWeb.Linq
{
    /// <summary>Provides an LINQ compatible access to the triple store.</summary>
    /// <typeparam name="T">Type of entities to be queried.</typeparam>
    public class EntityQueryable<T>:QueryableBase<T>
    {
        /// <summary>Creates an instance of the queryable entity factory.</summary>
        /// <param name="entityContext">Entity context to be used by this provider.</param>
        /// <param name="entitySource">Entity source.</param>
        /// <param name="mappingsRepository">Repository of mappings to be used when resolving strong types to ontologies.</param>
        /// <param name="baseUriSelectionPolicy">Base Uri selection policy to resolve relative Uris.</param>
        public EntityQueryable(IEntityContext entityContext,IEntitySource entitySource,IMappingsRepository mappingsRepository,[AllowNull] IBaseUriSelectionPolicy baseUriSelectionPolicy)
            :base(QueryParser.CreateDefault(),new EntityQueryExecutor(entityContext,entitySource))
        {
        }

        /// <summary>Creates an instance of the queryable entity source.</summary>
        /// <param name="provider">Query provider to be used by this queryable instance.</param>
        /// <param name="expression">Expression to be parsed.</param>
        public EntityQueryable(IQueryProvider provider,Expression expression):base(provider,expression)
        {
        }
    }
}