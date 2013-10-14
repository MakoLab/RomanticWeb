using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	/// <summary>Executes queries against underlying triple store.</summary>
	internal class EntityQueryExecutor:IQueryExecutor
	{
		#region Fields
		private IEntityContext _entityContext;
		private IMappingsRepository _mappingsRepository;
		private IOntologyProvider _ontologyProvider;
		#endregion

		#region Constructors
		/// <summary>Creates an instance of the query executor aware of the entities queried.</summary>
		/// <param name="entityContext">Entity factory to be used when creating objects.</param>
		/// <param name="mappingsRepository">Mappings repository to resolve strongly typed properties and types.</param>
		/// <param name="ontologyProvider">Ontology provider with data scheme.</param>
		internal EntityQueryExecutor(IEntityContext entityContext,IMappingsRepository mappingsRepository,IOntologyProvider ontologyProvider)
			{
			_entityContext=entityContext;
			_mappingsRepository=mappingsRepository;
			_ontologyProvider=ontologyProvider;
		}
		#endregion

		#region Public methods
		/// <summary>Returns a resulting collection of a query.</summary>
		/// <typeparam name="T">Type of elements to be returned.</typeparam>
		/// <param name="queryModel">Query model to be parsed.</param>
		/// <returns>Enumeration of resulting entities matching given query.</returns>
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
				MethodInfo createMethodInfo=_entityContext.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Where(item =>
					(item.Name=="Load")&&(item.GetGenericArguments().Length==1)&&(item.GetParameters().Length==1)&&(item.GetParameters()[0].ParameterType==typeof(string))).FirstOrDefault();
				result=(IEnumerable<T>)createMethodInfo.MakeGenericMethod(new Type[] { typeof(T) }).Invoke(_entityContext,new object[] { commandText });
			}

			return result;
		}

		/// <summary>Returns a scalar value beeing a result of a query.</summary>
		/// <typeparam name="T">Type of element to be returned.</typeparam>
		/// <param name="queryModel">Query model to be parsed.</param>
		/// <returns>Single scalar value beeing result of a query.</returns>
		public T ExecuteScalar<T>(QueryModel queryModel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Returns a single entity beeing a result of a query.</summary>
		/// <typeparam name="T">Type of element to be returned.</typeparam>
		/// <param name="queryModel">Query model to be parsed.</param>
		/// <param name="returnDefaultWhenEmpty">Tells the executor to return a defalt value in case of an empty result.</param>
		/// <returns>Single entity beeing result of a query.</returns>
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
				MethodInfo createMethodInfo=_entityContext.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Where(item =>
					(item.Name=="Load")&&(item.GetGenericArguments().Length==1)&&(item.GetParameters().Length==1)&&(item.GetParameters()[0].ParameterType==typeof(string))).FirstOrDefault();
				result=((IEnumerable<T>)createMethodInfo.MakeGenericMethod(new Type[] { typeof(T) }).Invoke(_entityContext,new object[] { commandText })).FirstOrDefault();
			}

			return result;
		}
		#endregion
	}
}