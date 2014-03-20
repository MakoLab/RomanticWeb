using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Dynamic
{
    internal class DynamicDictionaryMappingProvider
    {
        private IOntologyProvider _ontologyProvider;

        public EntityMap CreateDictionaryOwnerMapping(EntityMapping entityMapping,IDictionaryMappingProvider map)
        {
            var actualEntityType = map.PropertyInfo.DeclaringType;
            var owner = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Owner", actualEntityType.FullName, map.PropertyInfo.Name));
            var entry = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Entry", actualEntityType.FullName, map.PropertyInfo.Name));
            var type=typeof(DictionaryOwnerMap<,,,>);
            var typeArguments=new[] { owner,entry }.Concat(map.PropertyInfo.PropertyType.GenericTypeArguments).ToArray();
            var ownerMapType=type.MakeGenericType(typeArguments);

            AssemblyName asmName = new AssemblyName { Name = "HelloWorld" };

            AssemblyBuilder assemblyBuilder =
                Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            var defineDynamicModule=assemblyBuilder.DefineDynamicModule("DynamicMappings");
            var typeBuilderHelper = defineDynamicModule.DefineType(owner.Name + "Map",TypeAttributes.Public, ownerMapType);
            var methodBuilderHelper = typeBuilderHelper.DefineMethod("SetupEntriesCollection", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), new[] { typeof(ITermPart<CollectionMap>) });

            var ilGenerator=methodBuilderHelper.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, entityMapping.PropertyFor(map.PropertyInfo.Name).Uri.ToString());
            ilGenerator.Emit(OpCodes.Newobj,typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt,typeof(ITermPart<CollectionMap>).GetMethod("Is",new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);

            return (EntityMap)Activator.CreateInstance(typeBuilderHelper.CreateType());
        }

        public EntityMap CreateDictionaryEntryMapping(EntityMapping entityMapping, IDictionaryMappingProvider map)
        {
            var actualEntityType = map.PropertyInfo.DeclaringType;
            var entry = actualEntityType.Assembly.GetType(string.Format("{0}_{1}_Entry", actualEntityType.FullName, map.PropertyInfo.Name));
            var type = typeof(DictionaryEntryMap<,,>);
            var typeArguments = new[] { entry }.Concat(map.PropertyInfo.PropertyType.GenericTypeArguments).ToArray();
            var ownerMapType = type.MakeGenericType(typeArguments);

             AssemblyName asmName = new AssemblyName { Name="HelloWorld" };

            AssemblyBuilder assemblyBuilder =
                Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            var defineDynamicModule = assemblyBuilder.DefineDynamicModule("DynamicMappings");
            var typeBuilderHelper = defineDynamicModule.DefineType(entry.Name + "Map",TypeAttributes.Public, ownerMapType);
            var methodBuilderHelper = typeBuilderHelper.DefineMethod("SetupKeyProperty", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), new[] { typeof(ITermPart<PropertyMap>) });

            var ilGenerator = methodBuilderHelper.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldstr, map.Key.GetTerm(_ontologyProvider).ToString());
            ilGenerator.Emit(OpCodes.Newobj, typeof(Uri).GetConstructor(new[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Callvirt, typeof(ITermPart<PropertyMap>).GetMethod("Is", new Type[] { typeof(Uri) }));
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);

            var methodBuilderHelper1 = typeBuilderHelper.DefineMethod("SetupValueProperty", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), new[] { typeof(ITermPart<PropertyMap>) });

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