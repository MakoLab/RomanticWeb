using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
    public class Queryable<T> : QueryableBase<T>
    {
        public Queryable(IEntityContext entityContext,IEntitySource entitySource,IMappingsRepository mappings,IOntologyProvider ontologyProvider)
            :base(QueryParser.CreateDefault(),new QueryExecutor(entityContext,entitySource,mappings,ontologyProvider))
        {
        }

        public Queryable(IQueryProvider provider,Expression expression)
            :base(provider,expression)
        {
        }
    }
}