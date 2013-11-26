using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Mappings repository, which reads mapping attributes from an assembly
    /// </summary>
    public sealed class AttributeMappingsRepository:AssemblyMappingsRepository
    {
        /// <summary>
        /// Creates a new instance of <see cref="AttributeMappingsRepository"/>
        /// </summary>
        public AttributeMappingsRepository(Assembly assembly):base(assembly)
        {
        }

        #region Private methods

        /// <summary>
        /// Finds all types annotated with mapping attributes and build mappings
        /// </summary>
        protected override IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(MappingContext mappingContext)
        {
            return from type in Assembly.GetTypes()
                   let entityAttributes = type.GetCustomAttributes(typeof(EntityAttribute), true)
                   let classAttributes=type.GetCustomAttributes(typeof(ClassAttribute),true)
                   where entityAttributes.Any() || classAttributes.Any()
                   let singleOrDefault = classAttributes.Cast<ClassAttribute>().SingleOrDefault()
                   let classMapping = singleOrDefault != null ? singleOrDefault.GetMapping(mappingContext) :null
                   let propertyMappings = BuildPropertyMappings(type,mappingContext).ToList() 
                   select new Tuple<Type,IEntityMapping>(type,new EntityMapping { Class=classMapping,Properties=propertyMappings });
        }

        private static IEnumerable<IPropertyMapping> BuildPropertyMappings(Type type, MappingContext ontologyProvider)
        {
            var inheritedProperties = from inherited in type.GetInterfaces()
                                      from property in inherited.GetProperties()
                                      select property;
            return from property in type.GetProperties().Union(inheritedProperties)
                   from mapping in property.GetCustomAttributes(typeof(PropertyAttribute), true).Cast<PropertyAttribute>()
                   let propertyMapping = mapping.GetMapping(property.PropertyType, property.Name, ontologyProvider)
                   select propertyMapping;
        }

        #endregion
    }
}