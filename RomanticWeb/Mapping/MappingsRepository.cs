using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullGuard;
using RomanticWeb.Dynamic;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping
{
    public sealed class MappingsRepository : IMappingsRepository
    {
        private readonly object _locker=new object();
        private readonly IDictionary<Tuple<Assembly,Type>,IMappingSource> _sources;
        private readonly IDictionary<Type, IEntityMapping> _mappings;

        public MappingsRepository()
        {
            _sources=new Dictionary<Tuple<Assembly,Type>,IMappingSource>();
            _mappings = new Dictionary<Type, IEntityMapping>();
        }

        internal IEnumerable<IMappingSource> Sources
        {
            get
            {
                return _sources.Values;
            }
        }

        public void RebuildMappings(MappingContext mappingContext)
        {
            var conventionsVisitor=new ConventionsVisitor(mappingContext.Conventions);
            var mappingBuilder=new MappingModelBuilder(mappingContext);
            var dictionaryMappingProvider=new DynamicDictionaryMappingSource(mappingContext.OntologyProvider);

            var providers=new Dictionary<Type,IList<IEntityMappingProvider>>();
            CreateStaticMappings(providers,conventionsVisitor,dictionaryMappingProvider);
            CreateDynamicMappings(providers,dictionaryMappingProvider,conventionsVisitor);

            var singleProviderPerType=providers.Select(provider => provider.Value.Count>1?new MultiMappingProvider(provider.Key,provider.Value):provider.Value[0]).ToList();

            var inheriatenceBuilder=new InheritanceMappingBuilder(singleProviderPerType);
            foreach (var provider in inheriatenceBuilder.CombineInheritingMappings())
            {
                StoreMapping(mappingBuilder.BuildMapping(provider));
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

        /// <inheritdoc />
        [return: AllowNull]
        public PropertyInfo MappingForProperty(Uri predicateUri)
        {
            return (from mapping in _mappings
                    let type=mapping.Value
                    from property in type.Properties
                    where property.Uri.AbsoluteUri==predicateUri.AbsoluteUri
                    let propertyMapping=type.EntityType.GetProperty(property.Name,BindingFlags.Public|BindingFlags.Instance)
                    where propertyMapping!=null
                    select propertyMapping).FirstOrDefault();
        }

        public void AddSource(Assembly mappingAssembly,IMappingSource mappingSource)
        {
            lock (_locker)
            {
                var key=Tuple.Create(mappingAssembly,mappingSource.GetType());
                
                if (!_sources.ContainsKey(key))
                {
                    _sources.Add(key,mappingSource);
                }
            }
        }

        private void CreateStaticMappings(
            IDictionary<Type,IList<IEntityMappingProvider>> providers,
            params IMappingProviderVisitor[] visitors)
        {
            CreateMappings(providers,Sources,visitors);
        }

        private void CreateDynamicMappings(
            IDictionary<Type, IList<IEntityMappingProvider>> providers,
            IMappingSource source,
            params IMappingProviderVisitor[] visitors)
        {
            CreateMappings(providers,new[] { source },visitors);
        }

        private void CreateMappings(
            IDictionary<Type, IList<IEntityMappingProvider>> providers,
            IEnumerable<IMappingSource> sources,
            params IMappingProviderVisitor[] visitors)
        {
            foreach (var provider in sources.SelectMany(mappingSource => mappingSource.GetMappingProviders()))
            {
                foreach (var visitor in visitors)
                {
                    provider.Accept(visitor);
                }

                if (!providers.ContainsKey(provider.EntityType))
                {
                    providers[provider.EntityType] = new List<IEntityMappingProvider>();
                }

                providers[provider.EntityType].Add(provider);
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