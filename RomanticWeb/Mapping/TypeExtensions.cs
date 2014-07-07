using System;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Mapping
{
    internal static class TypeExtensions
    {
        public static bool IsConstructableEntityMap(this Type mappingType)
        {
            var hasConstructor = new Lazy<bool>(() => HasParameterlessConstructor(mappingType));
            return (typeof(EntityMap).IsAssignableFrom(mappingType)) && (!mappingType.IsAbstract) && (hasConstructor.Value) && (!mappingType.IsGenericTypeDefinition);
        }

        public static PropertyInfo FindProperty(this Type type, string name)
        {
            var property = GetProperty(type, name);

            if (property == null && type.IsInterface)
            {
                property = type.GetInterfaces().Select(iface => GetProperty(iface, name)).FirstOrDefault();
            }

            return property;
        }

        private static PropertyInfo GetProperty(Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
        }

        private static bool HasParameterlessConstructor(Type mappingType)
        {
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return mappingType.GetConstructor(Flags, null, Type.EmptyTypes, null) != null;
        }
    }
}