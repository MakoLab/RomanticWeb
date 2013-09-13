using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;

namespace RomanticWeb.Linq
{
	internal class EntityQueryExecutor:IQueryExecutor
	{
		private IEntityFactory _entityFactory;
		private EntitySparqlQueryModelTranslator _translator;

		internal EntityQueryExecutor(IEntityFactory entityFactory)
		{
			if (entityFactory==null)
				throw new ArgumentNullException("entityFactory");
			_entityFactory=entityFactory;
			_translator=new EntitySparqlQueryModelTranslator(_entityFactory);
		}

		public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
		{
			if ((!typeof(IEntity).IsAssignableFrom(typeof(T)))&&(typeof(T).IsValueType))
				throw new ArgumentOutOfRangeException("T");
			IEnumerable<T> result=new T[0];
			string commandText=_translator.CreateCommandText(queryModel);
			if (commandText.Length>0)
			{
				MethodInfo createMethodInfo=_entityFactory.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Where(item => 
					(item.Name=="Create")&&(item.GetGenericArguments().Length==1)&&(item.GetParameters().Length==1)&&(item.GetParameters()[0].ParameterType==typeof(string))).FirstOrDefault();
				result=(IEnumerable<T>)createMethodInfo.MakeGenericMethod(new Type[] { typeof(T) }).Invoke(_entityFactory,new object[] { commandText });
			}
			return result;
		}

		public T ExecuteScalar<T>(QueryModel queryModel)
		{
			throw new NotImplementedException();
		}

		public T ExecuteSingle<T>(QueryModel queryModel,bool returnDefaultWhenEmpty)
		{
			throw new NotImplementedException();
		}
	}
}