using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	/// <summary>Serves as a query provider for the triple store and its entities.</summary>
	/// <typeparam name="T"></typeparam>
	public class EntityQueryProvider<T>:QueryProviderBase where T:class,IEntity
	{
		#region Fields
		private IEntityFactory _entityFactory;
		private IMappingsRepository _mappingsRepository;
		private IOntologyProvider _ontologyProvider;
		#endregion

		#region Constructors
		/// <summary>Creates an instance of the query provider.</summary>
		/// <param name="entityFactory">Entity factory to be used when creating objects.</param>
		/// <param name="mappingsRepository">Mappings repository used to resolve strongly tuped properties and types.</param>
		/// <param name="ontologyProvider">Ontology provider that holds the data scheme.</param>
		protected internal EntityQueryProvider(IEntityFactory entityFactory,IMappingsRepository mappingsRepository,IOntologyProvider ontologyProvider):
			base(EntityQueryProvider<T>.CreateDefaultQueryParser(),new EntityQueryExecutor(entityFactory,mappingsRepository,ontologyProvider))
		{
			if (entityFactory==null)
			{
				throw new ArgumentNullException("entityFactory");
			}

			if (mappingsRepository==null)
			{
				throw new ArgumentNullException("mappingsRepository");
			}

			if (ontologyProvider==null)
			{
				throw new ArgumentNullException("ontologyProvider");
			}

			if (!typeof(IEntity).IsAssignableFrom(typeof(T)))
			{
				ExceptionHelper.ThrowGenericArgumentOutOfRangeException("T",typeof(Entity),typeof(T));
			}

			_entityFactory=entityFactory;
			_mappingsRepository=mappingsRepository;
			_ontologyProvider=ontologyProvider;
		}
		#endregion

		#region Public methods
		/// <summary>Creates an queryable enumeration for given expression.</summary>
		/// <typeparam name="T">Type of entities to be returned.</typeparam>
		/// <param name="expression">Expression to be used as a source for the query.</param>
		/// <returns>Queryable enumeration of entities.</returns>
		public override IQueryable<T> CreateQuery<T>(Expression expression)
		{
			Type genericQueryable=typeof(EntityQueryable<>).MakeGenericType(new Type[] { typeof(T) });
			ConstructorInfo constructorInfo=genericQueryable.GetConstructor(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance,null,new Type[] { typeof(IEntityFactory),typeof(IQueryProvider),typeof(Expression) },null);
			if (constructorInfo==null)
			{
				ExceptionHelper.ThrowGenericArgumentOutOfRangeException("T",typeof(Entity),typeof(T));
			}

			return (IQueryable<T>)constructorInfo.Invoke(new object[] { _entityFactory,this,expression });
		}
		#endregion

		#region Private methods
		private static QueryParser CreateDefaultQueryParser()
		{
			return new QueryParser(CreateDefaultExpressionTreeParser());
		}

		private static ExpressionTreeParser CreateDefaultExpressionTreeParser()
		{
			return new ExpressionTreeParser(ExpressionTreeParser.CreateDefaultNodeTypeProvider(),ExpressionTreeParser.CreateDefaultProcessor(ExpressionTransformerRegistry.CreateDefault()));
		}
		#endregion
	}
}