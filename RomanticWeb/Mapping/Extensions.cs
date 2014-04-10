using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides useful mappings repository extension methods.</summary>
    public static class Extensions
    {
        private static readonly Type EntityType=typeof(IEntity);
        private static readonly Type ObjectType=typeof(object);

        /// <summary>Searches for class mappings.</summary>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <param name="type">Type of entity.</param>
        /// <returns>Class mapping or null.</returns>
        public static IEnumerable<Uri> FindMappedClasses(this IMappingsRepository mappingsRepository,Type type)
        {
            IEnumerable<Uri> result=new Uri[0];
            if ((mappingsRepository!=null)&&(type!=null))
            {
                type=type.FindEntityType();
                if (EntityType.IsAssignableFrom(type))
                {
                    IEntityMapping entityMapping=mappingsRepository.FindEntityMapping(type);
                    if (entityMapping!=null)
                    {
                        result=entityMapping.Classes.OfType<IQueryableClassMapping>().SelectMany(cm=>cm.Uris).Distinct(AbsoluteUriComparer.Default);
                    }
                }
            }

            return result;
        }

        /// <summary>Searches for class mappings.</summary>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <returns>Class mapping or null.</returns>
        public static IEnumerable<Uri> FindMappedClasses<T>(this IMappingsRepository mappingsRepository)
        {
            return mappingsRepository.FindMappedClasses(typeof(T));
        }

        /// <summary>Searches for property mappings.</summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <param name="propertyName">Property name to be searched for.</param>
        /// <returns>Property mapping or null.</returns>
        [return: AllowNull]
        public static IPropertyMapping FindPropertyMapping<T>(this IMappingsRepository mappingsRepository,string propertyName) where T:IEntity
        {
            return mappingsRepository.FindPropertyMapping(typeof(T),propertyName);
        }

        /// <summary>Searches for property mappings.</summary>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <param name="property">Property to be searched for.</param>
        /// <returns>Property mapping or null.</returns>
        [return: AllowNull]
        public static IPropertyMapping FindPropertyMapping(this IMappingsRepository mappingsRepository,PropertyInfo property)
        {
            return mappingsRepository.FindPropertyMapping(property.DeclaringType,property.Name);
        }

        /// <summary>Searches for property mappings.</summary>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <param name="declaringType">Type of entity.</param>
        /// <param name="propertyName">Property name to be searched for.</param>
        /// <returns>Property mapping or null.</returns>
        [return: AllowNull]
        public static IPropertyMapping FindPropertyMapping(this IMappingsRepository mappingsRepository,Type declaringType,string propertyName)
        {
            IEntityMapping entityMapping=mappingsRepository.FindEntityMapping(declaringType);
            IPropertyMapping result=null;
            if (entityMapping!=null)
            {
                result=entityMapping.PropertyFor(propertyName);
            }

            return result;
        }

        /// <summary>Searches for entity mappings.</summary>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <param name="type">Type of entity.</param>
        /// <returns>Entity mapping or null.</returns>
        [return: AllowNull]
        public static IEntityMapping FindEntityMapping(this IMappingsRepository mappingsRepository,Type type)
        {
            if (!EntityType.IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException("type");
            }

            return mappingsRepository.MappingFor(type);
        }

        /// <summary>Searches for entity mappings.</summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="mappingsRepository">Repository to be queried.</param>
        /// <returns>Entity mapping or null.</returns>
        [return: AllowNull]
        public static IEntityMapping FindEntityMapping<T>(this IMappingsRepository mappingsRepository) where T:IEntity
        {
            return mappingsRepository.FindEntityMapping(typeof(T));
        }

        /// <summary>Searches for IEntity based type.</summary>
        /// <param name="type">Type to be searched through.</param>
        /// <returns><see cref="RomanticWeb.Entities.IEntity" /> based type or <b>null</b>.</returns>
        [return: AllowNull]
        public static Type FindEntityType(this Type type)
        {
            Type result=EntityTypeSanityCheck(type);
            if ((result!=null)&&(!typeof(IEntity).IsAssignableFrom(result)))
            {
                result=null;
                Type resultType=type;
                if (resultType.IsGenericType)
                {
                    result=FindEntityType(resultType.GetGenericArguments());
                }

                if (result==null)
                {
                    if ((resultType.IsArray)&&(!EntityType.IsAssignableFrom(result=resultType.GetElementType())))
                    {
                        result=null;
                    }
                }

                if (result==null)
                {
                    result=FindEntityType(resultType.GetInterfaces());
                }

                if (result==null)
                {
                    if (resultType.BaseType!=ObjectType)
                    {
                        result=resultType.BaseType.FindEntityType();
                    }
                }
            }

            return result;
        }

        [return: AllowNull]
        private static Type FindEntityType(IEnumerable<Type> types)
        {
            return types.FirstOrDefault(type => EntityType.IsAssignableFrom(type));
        }

        [return: AllowNull]
        private static Type EntityTypeSanityCheck(Type type)
        {
            switch (type.FullName)
            {
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                case "System.DateTime":
                case "System.TimeSpan":
                case "System.Char":
                case "System.String":
                    return null;
                default:
                    return type;
            }
        }
    }
}