using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
    using RomanticWeb.Mapping;
    using RomanticWeb.Ontologies;

	internal class EntityQueryExecutor:IQueryExecutor
	{
		#region Fields
		private IEntityFactory _entityFactory;
		private IMappingsRepository _mappingsRepository;
		private IOntologyProvider _ontologyProvider;
		#endregion

		#region Constructors
        internal EntityQueryExecutor(IEntityFactory entityFactory,IMappingsRepository mappings,IOntologyProvider ontologyProvider)
			{
			_entityFactory=entityFactory;
			_mappingsRepository=mappingsRepository;
			_ontologyProvider=ontologyProvider;
		}
		#endregion

		#region Public methods
		public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
		{
			if ((!typeof(IEntity).IsAssignableFrom(typeof(T)))&&(typeof(T).IsValueType))
			{
				ExceptionHelper.ThrowGenericArgumentOutOfRangeException("T",typeof(IEntity),typeof(T));
			}

			IEnumerable<T> result=new T[0];
			string commandText=EntityQueryModelVisitor.CreateCommandText(queryModel,_ontologyProvider,_mappingsRepository);
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
			if ((!typeof(IEntity).IsAssignableFrom(typeof(T)))&&(typeof(T).IsValueType))
			{
				ExceptionHelper.ThrowGenericArgumentOutOfRangeException("T",typeof(IEntity),typeof(T));
			}

			T result=default(T);
			string commandText=EntityQueryModelVisitor.CreateCommandText(queryModel,_ontologyProvider,_mappingsRepository);
			if (commandText.Length>0)
			{
				MethodInfo createMethodInfo=_entityFactory.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Where(item =>
					(item.Name=="Create")&&(item.GetGenericArguments().Length==1)&&(item.GetParameters().Length==1)&&(item.GetParameters()[0].ParameterType==typeof(string))).FirstOrDefault();
				result=((IEnumerable<T>)createMethodInfo.MakeGenericMethod(new Type[] { typeof(T) }).Invoke(_entityFactory,new object[] { commandText })).FirstOrDefault();
			}

			return result;
		}
		#endregion
	}
}