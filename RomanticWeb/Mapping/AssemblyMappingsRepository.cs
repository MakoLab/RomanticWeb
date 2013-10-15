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
        private IOntologyProvider _ontologyProvider;
        private IDictionary<Type,IEntityMapping> _mappings;
        #endregion

        #region Constructors
        /// <summary>Default constructor with given ontology provider to be used when resolving prefixes.</summary>
        /// <param name="ontologyProvider">Ontology provider to be used to resolve prefixes.</param>
        public AssemblyMappingsRepository(IOntologyProvider ontologyProvider)
        {
            ((IMappingsRepository)this).OntologyProvider=ontologyProvider;
        }

        /// <summary>Default parametereles constructor.</summary>
        internal AssemblyMappingsRepository()
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets an ontology provider to be used by this mappings repository to resolve namespace prefixes.</summary>
        IOntologyProvider IMappingsRepository.OntologyProvider
        {
            get
            {
                return _ontologyProvider;
            }

            set
            {
                _ontologyProvider=value;
                _mappings=AppDomain.CurrentDomain.GetAssemblies().SelectMany(BuildTypeMappings).ToDictionary(item => item.Item1,item => item.Item2);
            }
        }
        #endregion

        #region Public methods
        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        [return: AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            return (_mappings.ContainsKey(typeof(TEntity))?_mappings[typeof(TEntity)]:null);
        }
        #endregion

        #region Private methods
        private IEnumerable<IPropertyMapping> BuildPropertyMappings(Type type)
        {
            return from property in type.GetProperties()
                   from mapping in property.GetCustomAttributes(typeof(PropertyAttribute),true).Cast<PropertyAttribute>()
                   select mapping.GetMapping(property.Name,_ontologyProvider);
        }

        private IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(Assembly assembly)
        {
            return from type in assembly.GetTypes()
                   from mapping in type.GetCustomAttributes(typeof(ClassAttribute),true).Cast<ClassAttribute>()
                   let classMapping=mapping.GetMapping(_ontologyProvider)
                   let propertyMappings=BuildPropertyMappings(type).ToList()
                   where (classMapping!=null)&&(propertyMappings.Count>0)
                   select new Tuple<Type,IEntityMapping>(type,new EntityMapping() { Class=classMapping, Properties=propertyMappings });
        }
        #endregion
    }
}