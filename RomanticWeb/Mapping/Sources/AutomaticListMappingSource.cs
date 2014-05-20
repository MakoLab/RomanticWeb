using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using RomanticWeb.Collections;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Sources
{
    internal class AutomaticListMappingSource:IMappingProviderVisitor,IMappingProviderSource
    {
        private readonly IFluentMapsVisitor _visitor=new FluentMappingProviderBuilder();
        private readonly IDictionary<int,EntityMap> _entityMaps=new Dictionary<int,EntityMap>();
        private readonly IOntologyProvider _ontologyProvider;

        public AutomaticListMappingSource(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider=ontologyProvider;
            _entityMaps[typeof(IEntity).GetHashCode()^typeof(object).GetHashCode()^typeof(INodeConverter).GetHashCode()]=
                new ListEntryMap<IEntity,INodeConverter,IRdfListNode<IEntity,INodeConverter,dynamic>,dynamic>(null);
        }

        public IEnumerable<IEntityMappingProvider> GetMappingProviders()
        {
            return from map in _entityMaps select map.Value.Accept(_visitor);
        }

        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
            if (collectionMappingProvider.StoreAs==Model.StoreAs.RdfList)
            {
                int key=collectionMappingProvider.PropertyInfo.DeclaringType.GetHashCode()^
                    collectionMappingProvider.PropertyInfo.PropertyType.FindItemType().GetHashCode()^
                    (collectionMappingProvider.ElementConverterType!=null?collectionMappingProvider.ElementConverterType.GetHashCode():
                        (collectionMappingProvider.ConverterType!=null?collectionMappingProvider.ConverterType.GetHashCode():0));
                if (!_entityMaps.ContainsKey(key))
                {
                    _entityMaps.Add(key,CreateListEntryMapping(collectionMappingProvider));
                }
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
        }

        private EntityMap CreateDictionaryOwnerMapping(IDictionaryMappingProvider map)
        {
            // todo: refactoring
            var actualEntityType=map.PropertyInfo.DeclaringType;
            var owner=actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Owner",actualEntityType.FullName,map.PropertyInfo.Name));
            var entry=actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Entry",actualEntityType.FullName,map.PropertyInfo.Name));
            var type=typeof(DictionaryOwnerMap<,,,>);
            var typeArguments=new[] { owner,entry }.Concat(map.PropertyInfo.PropertyType.GenericTypeArguments).ToArray();
            var ownerMapType=type.MakeGenericType(typeArguments);

            AssemblyName asmName=new AssemblyName { Name="RomanticWeb.Dynamic" };

            AssemblyBuilder assemblyBuilder=
                Thread.GetDomain().DefineDynamicAssembly(asmName,AssemblyBuilderAccess.RunAndSave);
            var defineDynamicModule=assemblyBuilder.DefineDynamicModule("DynamicMappings");
            var typeBuilderHelper=defineDynamicModule.DefineType(owner.Name+"Map",TypeAttributes.Public,ownerMapType);
            var methodBuilderHelper=typeBuilderHelper.DefineMethod("SetupEntriesCollection",MethodAttributes.Public|MethodAttributes.Virtual|MethodAttributes.HideBySig,typeof(void),new[] { typeof(ITermPart<ICollectionMap>) });

            var ilGenerator=methodBuilderHelper.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr,map.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj,typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt,typeof(ITermPart<CollectionMap>).GetMethod("Is",new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);

            return (EntityMap)Activator.CreateInstance(typeBuilderHelper.CreateType());
        }

        private EntityMap CreateListEntryMapping(ICollectionMappingProvider map)
        {
            Type converterType=map.ElementConverterType??map.ConverterType;
            var nodeType=typeof(IRdfListNode<,,>).MakeGenericType(map.PropertyInfo.DeclaringType,converterType??typeof(INodeConverter),map.PropertyInfo.PropertyType.FindItemType());
            return (EntityMap)typeof(ListEntryMap<,,,>)
                .MakeGenericType(map.PropertyInfo.DeclaringType,converterType??typeof(INodeConverter),nodeType,nodeType.GetGenericArguments()[2])
                .GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public,null,new Type[] { typeof(Type) },null)
                .Invoke(new object[] { converterType });
        }
    }
}