using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Dynamic;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Sources
{
    internal class GeneratedDictionaryMappingSource : IMappingProviderVisitor, IMappingProviderSource
    {
        private readonly IFluentMapsVisitor _visitor = new FluentMappingProviderBuilder();
        private readonly List<EntityMap> _entityMaps = new List<EntityMap>();
        private readonly IOntologyProvider _ontologyProvider;
        private readonly EmitHelper _emitHelper;

        public GeneratedDictionaryMappingSource(MappingContext mappingContext, EmitHelper emitHelper)
        {
            _emitHelper = emitHelper;
            _ontologyProvider = mappingContext.OntologyProvider;
        }

        public string Description
        {
            get
            {
                return "Dictionary mappings";
            }
        }

        public IEnumerable<IEntityMappingProvider> GetMappingProviders()
        {
            return from map in _entityMaps
                   select map.Accept(_visitor);
        }

        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
        }

        public void Visit(IPropertyMappingProvider propertyMappingProvider)
        {
        }

        public void Visit(IDictionaryMappingProvider dictionaryMappingProvider)
        {
            _entityMaps.Add(CreateDictionaryOwnerMapping(dictionaryMappingProvider));
            _entityMaps.Add(CreateDictionaryEntryMapping(dictionaryMappingProvider));
        }

        public void Visit(IClassMappingProvider classMappingProvider)
        {
        }

        public void Visit(IEntityMappingProvider entityMappingProvider)
        {
        }

        private EntityMap CreateDictionaryOwnerMapping(IDictionaryMappingProvider map)
        {
            // todo: refactoring
            var actualEntityType = map.PropertyInfo.DeclaringType;
            var owner = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Owner", actualEntityType.FullName, map.PropertyInfo.Name));
            var entry = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Entry", actualEntityType.FullName, map.PropertyInfo.Name));
            var type = typeof(DictionaryOwnerMap<,,,>);
            var typeArguments = new[] { owner, entry }.Concat(map.PropertyInfo.PropertyType.GenericTypeArguments).ToArray();
            var ownerMapType = type.MakeGenericType(typeArguments);

            var defineDynamicModule = _emitHelper.GetDynamicModule();
            Type mapType = null;
            lock (defineDynamicModule)
            {
                mapType = defineDynamicModule.GetOrEmitType(owner.Name + "Map", builder => EmitOwnerMap(map, builder, owner, ownerMapType));
            }

            return (EntityMap)Activator.CreateInstance(mapType);
        }

        private TypeBuilder EmitOwnerMap(IDictionaryMappingProvider map, ModuleBuilder defineDynamicModule, Type owner, Type ownerMapType)
        {
            var typeBuilderHelper = defineDynamicModule.DefineType(owner.Name + "Map", TypeAttributes.Public, ownerMapType);
            var methodBuilderHelper = typeBuilderHelper.DefineMethod(
                "SetupEntriesCollection",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(void),
                new[] { typeof(ITermPart<ICollectionMap>) });

            var ilGenerator = methodBuilderHelper.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, map.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt, typeof(ITermPart<CollectionMap>).GetMethod("Is", new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);
            return typeBuilderHelper;
        }

        private EntityMap CreateDictionaryEntryMapping(IDictionaryMappingProvider map)
        {
            var actualEntityType = map.PropertyInfo.DeclaringType;
            var entry = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Entry", actualEntityType.FullName, map.PropertyInfo.Name));
            var type = typeof(DictionaryEntryMap<,,>);
            var typeArguments = new[] { entry }.Concat(map.PropertyInfo.PropertyType.GenericTypeArguments).ToArray();
            var ownerMapType = type.MakeGenericType(typeArguments);

            var defineDynamicModule = _emitHelper.GetDynamicModule();
            Type mapType = null;
            lock (defineDynamicModule)
            {
                mapType = defineDynamicModule.GetOrEmitType(entry.Name + "Map", builder => EmitEntryMap(map, builder, entry, ownerMapType));
            }

            return (EntityMap)Activator.CreateInstance(mapType);
        }

        private TypeBuilder EmitEntryMap(
            IDictionaryMappingProvider map, ModuleBuilder defineDynamicModule, Type entry, Type ownerMapType)
        {
            var typeBuilderHelper = defineDynamicModule.DefineType(entry.Name + "Map", TypeAttributes.Public, ownerMapType);
            var setupKeyMethod = typeBuilderHelper.DefineMethod(
                "SetupKeyProperty",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(void),
                new[] { typeof(ITermPart<IPropertyMap>) });
            EmitSetupPropertyOverride(setupKeyMethod, map.Key);

            var setupValueMethod = typeBuilderHelper.DefineMethod(
                "SetupValueProperty",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(void),
                new[] { typeof(ITermPart<IPropertyMap>) });
            EmitSetupPropertyOverride(setupValueMethod, map.Value);
            
            return typeBuilderHelper;
        }

        private void EmitSetupPropertyOverride(MethodBuilder methodBuilder, IPredicateMappingProvider termMapping)
        {
            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, termMapping.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt, typeof(ITermPart<PropertyMap>).GetMethod("Is", new Type[] { typeof(Uri) }));
            if (termMapping.ConverterType != null)
            {
                ilGenerator.Emit(OpCodes.Callvirt, typeof(IPropertyMap).GetMethod("ConvertWith").MakeGenericMethod(termMapping.ConverterType));
            }

            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);  
        }
    }
}