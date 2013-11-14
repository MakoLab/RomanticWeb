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
                   from mapping in type.GetCustomAttributes(typeof(ClassAttribute),true).Cast<ClassAttribute>() 
                   let classMapping = mapping.GetMapping(mappingContext) 
                   let propertyMappings = BuildPropertyMappings(type,mappingContext).ToList() 
                   where (classMapping!=null)&&(propertyMappings.Count>0) 
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