using System;
using System.Collections.Generic;
using System.Reflection;

namespace RomanticWeb.Entities
{
    /// <summary>Provides useful extensions for entity identifiers.</summary>
    public static class EntityIdExtensions
    {
        private const string IsAlreadyAbsoluteExceptionFormat="{0} {1} is already absolute.";
        private static readonly IDictionary<Type,ConstructorInfo> Constructors=new Dictionary<Type,ConstructorInfo>();

        /// <summary>Makes an entity identifier absolute.</summary>
        /// <remarks>In case entity identifier is already absolute method checks if the current Uri has given base Uri. If this test fails exception of type <see cref="ArgumentException"/> is thrown.</remarks>
        /// <typeparam name="TEntityId">Type of the entity identifier.</typeparam>
        /// <param name="entityId">Relative or absolute entity identifier.</param>
        /// <param name="baseUri">Base Uri.</param>
        /// <returns>Entity id which is absolute.</returns>
        public static TEntityId MakeAbsolute<TEntityId>(this TEntityId entityId,Uri baseUri) where TEntityId:EntityId
        {
            TEntityId result=GetSelfAlreadyAbsolute(entityId,baseUri);
            if (result==null)
            {
                if (!Constructors.ContainsKey(typeof(TEntityId)))
                {
                    Constructors[typeof(TEntityId)]=typeof(TEntityId).GetConstructor(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly,null,new[] { typeof(Uri) },null);
                }

                if (Constructors[typeof(TEntityId)]!=null)
                {
                    result=(TEntityId)Constructors[typeof(TEntityId)].Invoke(new object[] { new Uri(baseUri,entityId.Uri) });
                }
                else
                {
                    throw new NotImplementedException(String.Format("Type '{0}' does not implement a constructor with parameter of type '{1}'.",typeof(TEntityId),typeof(Uri)));
                }
            }

            return result;
        }

        private static TEntityId GetSelfAlreadyAbsolute<TEntityId>(TEntityId entityId,Uri baseUri) where TEntityId:EntityId
        {
            if (entityId.Uri.IsAbsoluteUri)
            {
                AssertIdHasExpectedBaseUri(entityId,baseUri);
                return entityId;
            }

            return null;
        }

        private static void AssertIdHasExpectedBaseUri(EntityId id,Uri baseUri)
        {
            if (baseUri==null)
            {
                throw new ArgumentNullException("baseUri");
            }

            if (baseUri.IsBaseOf(id.Uri)==false)
            {
                throw new ArgumentException(String.Format(IsAlreadyAbsoluteExceptionFormat,id.GetType().Name,id));
            }
        }
    }
}