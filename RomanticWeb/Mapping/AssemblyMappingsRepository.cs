using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Mappings repository, which reads entityMapping attributes from assemblies
    /// </summary>
    public sealed class AssemblyMappingsRepository:IMappingsRepository
    {
        #region Fields
        private IDictionary<Type,IEntityMapping> _mappings;
        #endregion

        #region Public methods
        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        [return: AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            return (_mappings.ContainsKey(typeof(TEntity))?_mappings[typeof(TEntity)]:null);
        }

        public void RebuildMappings(IOntologyProvider ontologyProvider)
        {
            _mappings = AppDomain.CurrentDomain
                                 .GetAssemblies()
                                 .SelectMany(assembly=>BuildTypeMappings(assembly,ontologyProvider))
                                 .ToDictionary(item => item.Item1, item => item.Item2);
        }

        #endregion

        #region Private methods
        private static IEnumerable<IPropertyMapping> BuildPropertyMappings(Type type,IOntologyProvider ontologyProvider)
        {
            return from property in type.GetProperties()
                   from mapping in property.GetCustomAttributes(typeof(PropertyAttribute),true).Cast<PropertyAttribute>()
                   let propertyMapping = mapping.GetMapping(property.Name,ontologyProvider)
                   select propertyMapping;
        }

        private static IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(Assembly assembly,IOntologyProvider ontologyProvider)
        {
            return from type in assembly.GetTypes() 
                   from mapping in type.GetCustomAttributes(typeof(ClassAttribute),true).Cast<ClassAttribute>() 
                   let classMapping = mapping.GetMapping(ontologyProvider) 
                   let propertyMappings = BuildPropertyMappings(type,ontologyProvider).ToList() 
                   where (classMapping!=null)&&(propertyMappings.Count>0) 
                   select new Tuple<Type,IEntityMapping>(type,new EntityMapping { Class=classMapping,Properties=propertyMappings });
        }

        #endregion
    }
}