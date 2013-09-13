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

namespace RomanticWeb.Linq
{
	public class EntityQueryProvider<T>:QueryProviderBase where T:class,IEntity
	{
		private IEntityFactory _entityFactory;

		internal protected EntityQueryProvider(IEntityFactory entityFactory):base(new QueryParser(new ExpressionTreeParser(
				ExpressionTreeParser.CreateDefaultNodeTypeProvider(),ExpressionTreeParser.CreateDefaultProcessor(ExpressionTransformerRegistry.CreateDefault()))),
			new EntityQueryExecutor(entityFactory))
		{
			if (entityFactory==null)
				throw new ArgumentNullException("entityFactory");
			if (!typeof(Entity).IsAssignableFrom(typeof(T)))
				ThrowGenericArgumentOutOfRangeException();
			_entityFactory=entityFactory;
		}

		public override IQueryable<T> CreateQuery<T>(Expression expression)
		{
			ConstructorInfo constructorInfo=typeof(EntityQueryable<>).MakeGenericType(new Type[] { typeof(T) }).GetConstructor(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance,null,
				new Type[] { typeof(IEntityFactory),typeof(IQueryProvider),typeof(Expression) },null);
			if (constructorInfo==null)
				ThrowGenericArgumentOutOfRangeException();
			return (IQueryable<T>)constructorInfo.Invoke(new object[] { _entityFactory,this,expression });
		}

		private void ThrowGenericArgumentOutOfRangeException()
		{
			throw new ArgumentOutOfRangeException("T",System.String.Format("Expected '{0}' derived type, but found '{1}'.",typeof(Entity).FullName,typeof(T).FullName));		
		}
	}
}