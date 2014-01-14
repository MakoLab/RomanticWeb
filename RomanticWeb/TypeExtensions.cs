using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;

namespace System
{
    /// <summary>Exposes usefull <see cref="Type" /> extension methods.</summary>
    public static class TypeExtensions
    {
        /// <summary>Tries to resolve item type of complex types.</summary>
        /// <param name="type">Type to be resolved.</param>
        /// <returns>Collection item type or <b>null</b>.</returns>
        public static Type FindItemType(this Type type)
        {
            Type result=type;
            if (type.IsArray)
            {
                result=type.GetElementType();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (type.IsGenericType)
                {
                    result=type.GetGenericArguments()[0];
                }
                else
                {
                    type=typeof(object);
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
                instanceType.GetInterfaces().Any(interfaceType => type.IsAssignableFromSpecificGeneric(interfaceType)));
        }

        /// <summary>Retrieves generic arguments for given specific generic type used in context of a generic type definition.</summary>
        /// <param name="type">Generic type definition to be checked against.</param>
        /// <param name="instanceType">Specific generic type to be analyzed.</param>
        /// <returns>Array of <see cref="Type" /> with generic type arguments of given specific generic type in context of a generic type definition.</returns>
        public static Type[] GetGenericArgumentsFor(this Type type,[AllowNull] Type instanceType)
        {
            Type[] result=new Type[0];
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
    }
}