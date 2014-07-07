using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RomanticWeb.Collections;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Dynamic;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Sources
{
    internal class GeneratedListMappingSource : IMappingProviderVisitor, IMappingProviderSource
    {
        private readonly IFluentMapsVisitor _visitor = new FluentMappingProviderBuilder();
        private readonly IList<EntityMap> _entryMaps = new List<EntityMap>();
        private readonly IOntologyProvider _ontologyProvider;

        private Type _currentEntityType;

        public GeneratedListMappingSource(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider = ontologyProvider;
        }

        public IEnumerable<IEntityMappingProvider> GetMappingProviders()
        {
            return from map in _entryMaps select map.Accept(_visitor);
        }

        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
            if (collectionMappingProvider.StoreAs == Model.StoreAs.RdfList)
            {
                _entryMaps.Add(CreateListOwnerMapping(collectionMappingProvider));
                _entryMaps.Add(CreateListEntryMapping(collectionMappingProvider));
            }
        }

        public void Visit(IPropertyMappingProvider propertyMappingProvider)
        {
        }

        public void Visit(IDictionaryMappingProvider dictionaryMappingProvider)
        {
        }

        public void Visit(IClassMappingProvider classMappingProvider)
        {
        }

        public void Visit(IEntityMappingProvider entityMappingProvider)
        {
            _currentEntityType = entityMappingProvider.EntityType;
        }

        private EntityMap CreateListOwnerMapping(ICollectionMappingProvider map)
        {
            var defineDynamicModule = EmitHelper.GetDynamicModule("DynamicListMappings");
            var ownerTypeName = GetOwnerTypeName(map);

            var mapType = defineDynamicModule.GetOrEmitType(ownerTypeName + "Map", moduleBuilder => EmitOwnerMappingType(map, moduleBuilder, ownerTypeName));
            return (EntityMap)Activator.CreateInstance(mapType);
        }

        private TypeBuilder EmitOwnerMappingType(IPropertyMappingProvider map, ModuleBuilder defineDynamicModule, string ownerTypeName)
        {
            var owner = defineDynamicModule.DefineType(
                    ownerTypeName,
                    TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract,
                    null,
                    new[] { typeof(IRdfListOwner) }).CreateType();
            var ownerMapType = typeof(ListOwnerMap<>).MakeGenericType(new[] { owner });

            var mapBuilderHelper = defineDynamicModule.DefineType(ownerTypeName + "Map", TypeAttributes.Public, ownerMapType);
            var propertyBuilder = mapBuilderHelper.DefineProperty("ListPredicate", PropertyAttributes.None, typeof(Uri), null);
            var getMethod = mapBuilderHelper.DefineMethod(
                "get_ListPredicate",
                MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                typeof(Uri),
                Type.EmptyTypes);
            propertyBuilder.SetGetMethod(getMethod);

            var ilGenerator = getMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldstr, map.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Ret);

            return mapBuilderHelper;
        }

        private EntityMap CreateListEntryMapping(ICollectionMappingProvider map)
        {
            var defineDynamicModule = EmitHelper.GetDynamicModule("DynamicListMappings");
            var nodeTypeName = GetNodeTypeName(map);

            var mapType = defineDynamicModule.GetOrEmitType(nodeTypeName + "Map", builder => EmitNodeMappingType(builder, map, nodeTypeName));

            return (EntityMap)Activator.CreateInstance(mapType);
        }

        private TypeBuilder EmitNodeMappingType(ModuleBuilder builder, ICollectionMappingProvider map, string nodeTypeName)
        {
            var elementType = map.PropertyInfo.PropertyType.FindItemType();
            var nodeType = builder.DefineType(
                   nodeTypeName,
                   TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract,
                   null,
                   new[] { typeof(IRdfListNode<>).MakeGenericType(elementType) }).CreateType();
            var converterType = map.ElementConverterType ?? map.ConverterType;
            var ownerMapType = typeof(ListEntryMap<,,>).MakeGenericType(nodeType, elementType, converterType);

            return builder.DefineType(nodeTypeName + "Map", TypeAttributes.Public, ownerMapType);
        }

        private string GetOwnerTypeName(ICollectionMappingProvider map)
        {
            return string.Format("{0}_{1}_ListOwner", _currentEntityType.FullName, map.PropertyInfo.Name);
        }

        private string GetNodeTypeName(ICollectionMappingProvider map)
        {
            return string.Format("{0}_{1}_ListNode", _currentEntityType.FullName, map.PropertyInfo.Name);
        }
    }
}