using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Sources
{
    internal class GeneratedDictionaryMappingSource:IMappingProviderVisitor,IMappingProviderSource
    {
        private readonly IFluentMapsVisitor _visitor=new FluentMappingProviderBuilder();
        private readonly List<EntityMap> _entityMaps=new List<EntityMap>();
        private readonly IOntologyProvider _ontologyProvider;

        public GeneratedDictionaryMappingSource(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider=ontologyProvider;
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

            AssemblyName asmName = new AssemblyName { Name = "RomanticWeb.Dynamic" };

            AssemblyBuilder assemblyBuilder =
                Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            var defineDynamicModule = assemblyBuilder.DefineDynamicModule("DynamicMappings");
            var typeBuilderHelper = defineDynamicModule.DefineType(owner.Name + "Map", TypeAttributes.Public, ownerMapType);
            var methodBuilderHelper = typeBuilderHelper.DefineMethod("SetupEntriesCollection", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), new[] { typeof(ITermPart<ICollectionMap>) });

            var ilGenerator = methodBuilderHelper.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, map.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt, typeof(ITermPart<CollectionMap>).GetMethod("Is", new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);

            return (EntityMap)Activator.CreateInstance(typeBuilderHelper.CreateType());
        }

        private EntityMap CreateDictionaryEntryMapping(IDictionaryMappingProvider map)
        {
            var actualEntityType = map.PropertyInfo.DeclaringType;
            var entry = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Entry", actualEntityType.FullName, map.PropertyInfo.Name));
            var type = typeof(DictionaryEntryMap<,,>);
            var typeArguments = new[] { entry }.Concat(map.PropertyInfo.PropertyType.GenericTypeArguments).ToArray();
            var ownerMapType = type.MakeGenericType(typeArguments);

            AssemblyName asmName = new AssemblyName { Name = "RomanticWeb.Dynamic" };

            AssemblyBuilder assemblyBuilder =
                Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            var defineDynamicModule = assemblyBuilder.DefineDynamicModule("DynamicMappings");
            var typeBuilderHelper = defineDynamicModule.DefineType(entry.Name + "Map", TypeAttributes.Public, ownerMapType);
            var methodBuilderHelper = typeBuilderHelper.DefineMethod("SetupKeyProperty", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), new[] { typeof(ITermPart<IPropertyMap>) });

            var ilGenerator = methodBuilderHelper.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, map.Key.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt, typeof(ITermPart<PropertyMap>).GetMethod("Is", new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);

            var methodBuilderHelper1 = typeBuilderHelper.DefineMethod("SetupValueProperty", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), new[] { typeof(ITermPart<IPropertyMap>) });

            ilGenerator = methodBuilderHelper1.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, map.Value.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt, typeof(ITermPart<PropertyMap>).GetMethod("Is", new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);

            return (EntityMap)Activator.CreateInstance(typeBuilderHelper.CreateType());
        }
    }
}