using System;
using System.Reflection;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Mapping
{
    internal static class TypeExtensions
    {
        public static bool IsConstructableEntityMap(this Type mappingType)
        {
            var hasConstructor=new Lazy<bool>(() => HasParameterlessConstructor(mappingType));
            return (typeof(EntityMap).IsAssignableFrom(mappingType))&&(!mappingType.IsAbstract)&&(hasConstructor.Value)&&(!mappingType.IsGenericTypeDefinition);
        }

        private static bool HasParameterlessConstructor(Type mappingType)
        {
            const BindingFlags Flags=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
            return mappingType.GetConstructor(Flags,null,Type.EmptyTypes,null)!=null;
        }
    }
}