using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    public abstract class MappingsRepositoryBase:IMappingsRepository
    {
        private readonly IDictionary<Type, IEntityMapping> _mappings;

        protected MappingsRepositoryBase()
        {
            _mappings=new Dictionary<Type,IEntityMapping>();
        }

        public void RebuildMappings(MappingContext mappingContext)
        {
            foreach (var mapping in CreateMappings(mappingContext))
            {
                foreach (var property in mapping.Properties)
                {
                    ApplyPropertyConventions(property, mappingContext.Conventions);
                }

                StoreMapping(mapping);
            }
        }

        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        [return: AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            return MappingFor(typeof(TEntity));
        }

        /// <inheritdoc />
        [return: AllowNull]
        public IEntityMapping MappingFor(Type entityType)
        {
            return (_mappings.ContainsKey(entityType) ? _mappings[entityType] : null);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public Type MappingFor(Uri classUri)
        {
            return (from mapping in _mappings
                    where mapping.Value.Classes.Any(item => item.Uri.AbsoluteUri == classUri.AbsoluteUri)
                    select mapping.Key).FirstOrDefault();
        }

        protected abstract IEnumerable<IEntityMapping> CreateMappings(MappingContext mappingContext); 

        private static void ApplyPropertyConventions(IPropertyMapping property, IEnumerable<IConvention> conventions)
        {
            var applicableConventions = from convention in conventions.OfType<IPropertyConvention>()
                                        where convention.ShouldApply(property)
                                        select convention;

            foreach (var convention in applicableConventions)
            {
                convention.Apply(property);
            }
        }

        private void StoreMapping(IEntityMapping mapping)
        {
            if (_mappings.ContainsKey(mapping.EntityType))
            {
                throw new MappingException(string.Format("Duplicate mapping for type {0}", mapping.EntityType));
            }

            _mappings.Add(mapping.EntityType, mapping);
        }
    }
}