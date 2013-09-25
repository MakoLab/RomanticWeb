using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Mappings repository, which reads entityMapping attributes from assemblies
    /// </summary>
    public sealed class AssemblyMappingsRepository : IMappingsRepository
    {
        #region Fields

        private readonly IOntologyProvider _ontologyProvider;

        private readonly IDictionary<Type,IEntityMapping> _mappings;
        #endregion

        #region Constructors
        internal AssemblyMappingsRepository(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider=ontologyProvider;
            _mappings=AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(BuildTypeMappings)
                               .ToDictionary(t => t.Item1,t => t.Item2);
        }

        #endregion

        #region Public methods

        public IEntityMapping MappingFor<TEntity>()
        {
            return _mappings[typeof(TEntity)];
        }

        #endregion

        #region Private methods

        private IEnumerable<IPropertyMapping> BuildPropertyMappings(Type type)
        {
            return from property in type.GetProperties()
                   from mapping in property.GetCustomAttributes(typeof(PropertyAttribute), true).Cast<PropertyAttribute>()
                   select mapping.GetMapping(property.Name, _ontologyProvider);
        }

        private IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(Assembly assembly)
		{
            return from type in assembly.GetTypes()
                   from mapping in type.GetCustomAttributes(typeof(RdfTypeAttribute),true).Cast<RdfTypeAttribute>()
                   let typeMapping = mapping.GetMapping(_ontologyProvider)
                   let propertyMapping = BuildPropertyMappings(type).ToList()
                   select new Tuple<Type,IEntityMapping>(type,new EntityMapping { Type=typeMapping,Properties=propertyMapping });
		}

        #endregion
    }
}