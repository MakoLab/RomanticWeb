using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullGuard;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;
using RomanticWeb.Entities;

namespace RomanticWeb.Linq
{
    // TODO: Make the QueryOptimizer passed from the IOC container.

    /// <summary>Executes queries against underlying triple store.</summary>
    public class EntityQueryExecutor : IQueryExecutor
    {
        #region Fields
        private static readonly MethodInfo EnumerableCastMethod = Info.OfMethod("System.Core", "System.Linq.Enumerable", "Cast", "IEnumerable");
        private static readonly MethodInfo EntityLoadMethod = Info.OfMethod("RomanticWeb", "RomanticWeb.IEntityContext", "Create", "EntityId");
        private readonly IEntityContext _entityContext;
        private readonly IEntitySource _entitySource;
        private readonly IQueryOptimizer _queryOptimizer;
        private readonly IEntityStore _store;
        private EntityQueryModelVisitor _modelVisitor;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the query executor aware of the entities queried.
        /// </summary>
        /// <param name="entityContext">Entity factory to be used when creating objects.</param>
        /// <param name="entitySource">Entity source.</param>
        /// <param name="store">Entity store</param>
        public EntityQueryExecutor(IEntityContext entityContext, IEntitySource entitySource, IEntityStore store)
        {
            _entityContext = entityContext;
            _entitySource = entitySource;
            _store = store;
            _queryOptimizer = new GenericQueryOptimizer();
        }
        #endregion

        #region Public methods
        /// <summary>Returns a scalar value beeing a result of a query.</summary>
        /// <typeparam name="T">Type of element to be returned.</typeparam>
        /// <param name="queryModel">Query model to be parsed.</param>
        /// <returns>Single scalar value beeing result of a query.</returns>
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var sparqlQuery = VisitQueryModel(queryModel);
            switch (sparqlQuery.QueryForm)
            {
                case Model.QueryForms.Ask: return (T)Convert.ChangeType(_entitySource.ExecuteAskQuery(sparqlQuery), typeof(T));
                default: return (T)Convert.ChangeType(_entitySource.ExecuteScalarQuery(sparqlQuery), typeof(T));
            }
        }

        /// <summary>Returns a single entity beeing a result of a query.</summary>
        /// <typeparam name="T">Type of element to be returned.</typeparam>
        /// <param name="queryModel">Query model to be parsed.</param>
        /// <param name="returnDefaultWhenEmpty">Tells the executor to return a default value in case of an empty result.</param>
        /// <returns>Single entity beeing result of a query.</returns>
        [return: AllowNull]
        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            if (queryModel.ResultOperators.OfType<FirstResultOperator>().Any())
            {
                return (returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).FirstOrDefault() : ExecuteCollection<T>(queryModel).First());
            }
            else
            {
                return (returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).SingleOrDefault() : ExecuteCollection<T>(queryModel).Single());
            }
        }

        /// <summary>Returns a resulting collection of a query.</summary>
        /// <typeparam name="T">Type of elements to be returned.</typeparam>
        /// <param name="queryModel">Query model to be parsed.</param>
        /// <returns>Enumeration of resulting entities matching given query.</returns>
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            IEnumerable<CastResultOperator> resultOperators = queryModel.ResultOperators.OfType<CastResultOperator>().ToArray();
            foreach (CastResultOperator resultOperator in resultOperators)
            {
                queryModel.ResultOperators.Remove(resultOperator);
            }

            Model.Query sparqlQuery = VisitQueryModel(queryModel);
            IEnumerable<EntityId> actualEntities;
            IEnumerable<RomanticWeb.Model.EntityQuad> quads = _entitySource.ExecuteEntityQuery(sparqlQuery, out actualEntities);
            IEnumerable<T> result = (!typeof(IEntity).IsAssignableFrom(typeof(T)) ? CreateLiteralResultSet<T>(quads, actualEntities) : CreateEntityResultSet<T>(quads, actualEntities));

            foreach (CastResultOperator resultOperator in resultOperators)
            {
                var castMethod = EnumerableCastMethod.MakeGenericMethod(resultOperator.CastItemType);
                result = (IEnumerable<T>)castMethod.Invoke(null, new object[] { result });
            }

            return result;
        }
        #endregion

        #region Non-public methods
        private Model.Query VisitQueryModel(QueryModel queryModel)
        {
            _modelVisitor = new EntityQueryModelVisitor(_entityContext);
            _modelVisitor.VisitQueryModel(queryModel);
            return _queryOptimizer.Optimize(_modelVisitor.Query);
        }

        private IEnumerable<T> CreateLiteralResultSet<T>(IEnumerable<RomanticWeb.Model.EntityQuad> quads, IEnumerable<EntityId> actualEntities)
        {
            IEnumerable<T> result;
            if (!(_modelVisitor.PropertyMapping is EntityQueryModelVisitor.IdentifierPropertyMapping))
            {
                result = quads.Select(triple => (T)_modelVisitor.PropertyMapping.Converter.Convert(triple.Object, _entityContext));
            }
            else
            {
                result = quads.Select(triple => (T)Convert.ChangeType(triple.Subject.Uri, typeof(T)));
            }

            return result;
        }

        private IEnumerable<T> CreateEntityResultSet<T>(IEnumerable<RomanticWeb.Model.EntityQuad> quads, IEnumerable<EntityId> actualEntities)
        {
            var groupedTriples = from triple in quads
                                 group triple by triple.EntityId into tripleGroup
                                 select tripleGroup;

            foreach (var triples in groupedTriples)
            {
                _store.AssertEntity(triples.Key, triples);
            }

            var createMethodInfo = EntityLoadMethod.MakeGenericMethod(new[] { typeof(T) });
            return actualEntities.Select(id => (T)createMethodInfo.Invoke(_entityContext, new object[] { id }));
        }
        #endregion
    }
}