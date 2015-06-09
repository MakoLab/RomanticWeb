using System.Collections.Generic;
using System.Reflection;
using RomanticWeb.LinkedData;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides a mapping lookup for <see cref="UrlMatchingResourceResolutionStrategy" />.</summary>
    public class BaseUriMappingModelVisitor : IMappingModelVisitor
    {
        private readonly ISet<Assembly> _mappingAssemblies = new HashSet<Assembly>();

        /// <summary>Gets the mapping assemblies.</summary>
        public IEnumerable<Assembly> MappingAssemblies { get { return _mappingAssemblies; } }

        /// <inheritdoc />
        public void Visit(IEntityMapping entityMapping)
        {
            if (!entityMapping.EntityType.Assembly.IsDynamic)
            {
                _mappingAssemblies.Add(entityMapping.EntityType.Assembly);
            }
        }

        /// <inheritdoc />
        public void Visit(ICollectionMapping collectionMapping)
        {
        }

        /// <inheritdoc />
        public void Visit(IDictionaryMapping dictionaryMapping)
        {
        }

        /// <inheritdoc />
        public void Visit(IPropertyMapping propertyMapping)
        {
        }

        /// <inheritdoc />
        public void Visit(IClassMapping classMapping)
        {
        }
    }
}