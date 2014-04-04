using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Entities;

namespace System
{
    /// <summary>Exposes useful <see cref="Type" /> extension methods.</summary>
    public static class TypeExtensions
    {
        /// <summary>Checks if the type can be assigned to the <see cref="IEnumerable" /> interface.</summary>
        /// <remarks>This method will return false for type <see cref="System.String" />.</remarks>
        /// <param name="type">Type to be checked.</param>
        /// <returns><b>true</b> if the type is <see cref="System.Array" /> or is assignable to <see cref="IEnumerable" /> (except <see cref="System.String" />); otherwise <b>false</b>.</returns>
        public static bool IsEnumerable(this Type type)
        {
            return (type!=null&&((type.IsArray)||((typeof(IEnumerable).IsAssignableFrom(type))&&(type!=typeof(string)))));
        }

        /// <summary>Tries to resolve item type of complex types.</summary>
        /// <param name="type">Type to be resolved.</param>
        /// <returns>Collection item type or <b>null</b>.</returns>
        public static Type FindItemType(this Type type)
        {
            Type result=type;
            if (type!=null)
            {
                if (type.IsArray)
                {
                    result=type.GetElementType();
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    result=type.GenericTypeArguments.Single();
                }
                else if ((typeof(IEnumerable).IsAssignableFrom(type))&&(type!=typeof(string)))
                {
                    if (type.IsGenericType)
                    {
                        result=type.GetGenericArguments()[0];
                    }
                    else
                    {
                        result=typeof(object);
                    }
                }
            }

            return result;
        }

        /// <summary>Checks if given generic type is assignable from other specific generic type.</summary>
        /// <param name="type">Type to assign to.</param>
        /// <param name="instanceType">Type to be assigned.</param>
        /// <returns><b>true</b> if specific generic type can be assigned to a generi type definition; otherwse <b>false</b>.</returns>
        public static bool IsAssignableFromSpecificGeneric(this Type type,[AllowNull] Type instanceType)
        {
            return (type!=null)&&(instanceType!=null)&&(instanceType!=typeof(object))&&
                (((instanceType.IsGenericType)&&(instanceType.GetGenericTypeDefinition()==type))||
                (type.IsAssignableFromSpecificGeneric(instanceType.BaseType))||
                instanceType.GetInterfaces().Any(type.IsAssignableFromSpecificGeneric));
        }

        /// <summary>Retrieves generic arguments for given specific generic type used in context of a generic type definition.</summary>
        /// <param name="type">Generic type definition to be checked against.</param>
        /// <param name="instanceType">Specific generic type to be analyzed.</param>
        /// <returns>Array of <see cref="Type" /> with generic type arguments of given specific generic type in context of a generic type definition.</returns>
        public static Type[] GetGenericArgumentsFor(this Type type,[AllowNull] Type instanceType)
        {
            var result=new Type[0];
            if ((type!=null)&&(instanceType!=null)&&(instanceType!=typeof(object)))
            {
                if ((instanceType.IsGenericType)&&(instanceType.GetGenericTypeDefinition()==type))
                {
                    result=instanceType.GetGenericArguments();
                }
                else if (!instanceType.GetInterfaces().Any(interfaceType => (result=type.GetGenericArgumentsFor(interfaceType)).Length>0))
                {
                    result=type.GetGenericArgumentsFor(instanceType.BaseType);
                }
            }

            return result.ToArray();
        }

        /// <summary>Changes item type in enumerable types.</summary>
        /// <param name="type">Enumerable type to change item type in.</param>
        /// <param name="newItemType">New item type.</param>
        /// <returns>New type with changed item type.</returns>
        public static Type ChangeItemType(this Type type,Type newItemType)
        {
            Type result=type;
            if (type!=null)
            {
                if (type.IsArray)
                {
                    result=Array.CreateInstance(newItemType,0).GetType();
                }
                else if (((typeof(IEnumerable).IsAssignableFrom(type))&&(type!=typeof(string)))&&(type.IsGenericType))
                {
                    result=type.GetGenericTypeDefinition().MakeGenericType(new[] { newItemType }.Union(type.GetGenericArguments().Skip(1)).ToArray());
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the direct parents of a type.
        /// </summary>
        internal static IEnumerable<Type> GetImmediateParents(this Type type,bool excludeIEntity=true)
        {
            if (type.BaseType!=null)
            {
                yield return type.BaseType;
            }

            var allInterfaces=type.GetInterfaces();
            foreach (var iface in allInterfaces.Except(allInterfaces.SelectMany(i => i.GetInterfaces())))
            {
                if (iface==typeof(IEntity)&&excludeIEntity)
                {
                    continue;
                }

                yield return iface;
            }

            if (type.IsGenericType)
            {
                var genericTypeDefinition=type.GetGenericTypeDefinition();

                if (genericTypeDefinition!=type)
                {
                    yield return genericTypeDefinition;
                }
            }
        }
    }
}