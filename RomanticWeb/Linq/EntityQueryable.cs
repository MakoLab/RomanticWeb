using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
    public class EntityQueryable<T> : QueryableBase<T>
    {
        #region Constructors
        public EntityQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
        }

        protected internal EntityQueryable(IEntityFactory entityFactory, IMappingsRepository mappings, IOntologyProvider ontologyProvider)
            : base(QueryParser.CreateDefault(), new EntityQueryExecutor(entityFactory, mappings, ontologyProvider))
        {
            if (!typeof(Entity).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentOutOfRangeException("T", System.String.Format("Expected '{0}' derived type, but found '{1}'.", typeof(Entity).FullName, typeof(T).FullName));
            }
        }

        protected internal EntityQueryable(IQueryProvider provider)
            : base(provider)
        {
        }
        #endregion
    }
}