using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Mapping.Validation;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Default implementation of <see cref="IMappingsRepository"/>
    /// </summary>
    public sealed class MappingsRepository:IMappingsRepository
    {
        private readonly object _locker=new object();
        private readonly IDictionary<Tuple<Assembly,Type>,IMappingProviderSource> _sources;
        private readonly IDictionary<Type,IEntityMapping> _mappings;
        private readonly IDictionary<Type,IEntityMappingProvider> _openGenericProviders;
        private IList<IMappingProviderVisitor> _providerVisitors;
        private MappingModelBuilder _mappingBuilder;
        private IList<IMappingModelVisitor> _visitors=new List<IMappingModelVisitor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingsRepository"/> class.
        /// </summary>
        internal MappingsRepository()
        {
            _sources=new Dictionary<Tuple<Assembly,Type>,IMappingProviderSource>();
            _mappings=new Dictionary<Type,IEntityMapping>();
            _openGenericProviders=new Dictionary<Type,IEntityMappingProvider>();
        }

        internal IEnumerable<IMappingProviderSource> Sources { get { return _sources.Values; } }

        /// <summary>
        /// Retrieves mapping providers from <see cref="IMappingProviderSource"/>s, 
        /// creates dynamic mappings, applies conventions and transforms providers into mappings
        /// </summary>
        public void RebuildMappings(MappingContext mappingContext)
        {
            _providerVisitors=GetDefaultProviderVisitors(mappingContext).ToArray();
            _mappingBuilder=new MappingModelBuilder(mappingContext);
            var modelVisitors=GetDefaultModelVisitors(mappingContext).Union(_visitors).ToArray();

            CreateMappings(Sources.ToArray(),modelVisitors);
            CreateMappings(_providerVisitors.OfType<IMappingProviderSource>().ToArray(),modelVisitors);
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
            if ((entityType.IsGenericType)&&(!entityType.IsGenericTypeDefinition))
            {
                var genericDefinition=entityType.GetGenericTypeDefinition();
                if (_openGenericProviders.ContainsKey(genericDefinition))
                {
                    return CreateMappingFromGenericDefinition(genericDefinition,entityType);
                }
            }

            return (_mappings.ContainsKey(entityType)?_mappings[entityType]:null);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public IPropertyMapping MappingForProperty(Uri predicateUri)
        {
            return (from mapping in _mappings
                    let type=mapping.Value
                    from property in type.Properties
                    where property.Uri.AbsoluteUri==predicateUri.AbsoluteUri
                    select property).FirstOrDefault();
        }

        internal void AddSource(Assembly mappingAssembly,IMappingProviderSource mappingSource)
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

        [Obsolete]
        internal void AddVisitor(IMappingModelVisitor mappingModelVisitor)
        {
            _visitors.Add(mappingModelVisitor);
        }

        private static IEnumerable<IMappingProviderVisitor> GetDefaultProviderVisitors(MappingContext context)
        {
            yield return new ConventionsVisitor(context);
            yield return new MappingProvidersValidator();
            yield return new AutomaticListMappingSource(context.OntologyProvider);
            yield return new GeneratedDictionaryMappingSource(context.OntologyProvider);
        }

        private static IEnumerable<IMappingModelVisitor> GetDefaultModelVisitors(MappingContext context)
        {
            yield break;
        }

        private void CreateMappings(IMappingProviderSource[] sources,IMappingModelVisitor[] modelVisitors)
        {
            var mappings=from source in sources
                         from provider in source.GetMappingProviders()
                         group provider by provider.EntityType into g
                         select new KeyValuePair<Type,IList<IEntityMappingProvider>>(g.Key,g.ToList());

            var singleProviderPerType=mappings.Select(provider => provider.Value.Count>1?new MultiMappingProvider(provider.Key,provider.Value):provider.Value[0]).ToList();

            var inheritanceMappingBuilder=new InheritanceMappingBuilder(singleProviderPerType);
            foreach (var provider in inheritanceMappingBuilder.CombineInheritingMappings().Where(p => p.Properties.Any()||p.Classes.Any()))
            {
                foreach (var visitor in _providerVisitors)
                {
                    provider.Accept(visitor);
                }

                if (provider.EntityType.IsGenericTypeDefinition)
                {
                    _openGenericProviders[provider.EntityType]=provider;
                }

                StoreMapping(_mappingBuilder.BuildMapping(provider),modelVisitors);
            }
        }

        private IEntityMapping CreateMappingFromGenericDefinition(Type genericDefinition,Type entityType)
        {
            var openGenericProvider=_openGenericProviders[genericDefinition];
            var provider=new ClosedGenericEntityMappingProvider(openGenericProvider,entityType.GenericTypeArguments);

            foreach (var visitor in _providerVisitors)
            {
                provider.Accept(visitor);
            }

            return _mappingBuilder.BuildMapping(provider);
        }

        private void StoreMapping(IEntityMapping mapping,IEnumerable<IMappingModelVisitor> modelVisitors)
        {
            if (_mappings.ContainsKey(mapping.EntityType))
            {
                throw new MappingException(string.Format("Duplicate mapping for type {0}",mapping.EntityType));
            }

            _mappings.Add(mapping.EntityType,mapping);

            foreach (var mappingModelVisitor in modelVisitors)
            {
                mapping.Accept(mappingModelVisitor);
            }
        }
    }
}