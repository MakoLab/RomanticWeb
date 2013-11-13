using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
    public class QueryExecutor:IQueryExecutor
    {
        private readonly IEntityContext _context;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly IOntologyProvider _ontologyProvider;
        private readonly QueryModelVisitor _modelVisitor;

        public QueryExecutor(IEntityContext context,IEntitySource entitySource,IMappingsRepository mappings,IOntologyProvider ontologyProvider)
        {
            _context=context;
            _entitySource=entitySource;
            _mappings=mappings;
            _ontologyProvider = ontologyProvider;
            _modelVisitor = new QueryModelVisitor(mappings);
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            throw new System.NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel,bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).SingleOrDefault() : ExecuteCollection<T>(queryModel).Single();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var createMethodInfo=Info.OfMethod("RomanticWeb", "RomanticWeb.IEntityContext", "Load", "EntityId,Boolean")
                                     .MakeGenericMethod(new[] { typeof(T) });
            
            ISet<EntityId> ids=new HashSet<EntityId>();
            var groupedTriples=from t in VisitAndExecuteEntityQuery(queryModel)
                               group t by new { t.EntityId } into g
                               select g;

            foreach (var triples in groupedTriples)
            {
                ids.Add(triples.Key.EntityId);
                _context.Store.AssertEntity(triples.Key.EntityId, triples);
            }

            return from id in ids 
                   select (T)createMethodInfo.Invoke(_context,new object[] { id,false });
        }

        private IEnumerable<EntityQuad> VisitAndExecuteEntityQuery(QueryModel queryModel)
        {
            _modelVisitor.VisitQueryModel(queryModel);
            return _entitySource.ExecuteEntityQuery(_modelVisitor.SparqlQuery);
        }
    }
}