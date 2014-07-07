using System;
using System.Reflection;
using System.Reflection.Emit;

namespace RomanticWeb.Dynamic
{
    internal static class EmitHelper
    {
        private static readonly object Locker = new object();
        private static readonly Lazy<AssemblyBuilder> AsmBuilder = new Lazy<AssemblyBuilder>(CreateBuilder);

        public static ModuleBuilder GetDynamicModule(string name)
        {
            lock (Locker)
            {
                var assemblyBuilder = GetBuilder();
                return assemblyBuilder.GetDynamicModule(name) ?? assemblyBuilder.DefineDynamicModule(name);
            }
        }

        public static AssemblyBuilder GetBuilder()
        {
            return AsmBuilder.Value;
        }

        public static Type GetOrEmitType(this ModuleBuilder moduleBuilder, string typeName, Func<ModuleBuilder, TypeBuilder> emitType)
        {
            Type mapType;
            if (moduleBuilder.GetType(typeName) != null)
            {
                mapType = moduleBuilder.GetType(typeName, true);
            }
            else
            {
                mapType = emitType(moduleBuilder).CreateType();
            }

            return mapType;
        }

        private static AssemblyBuilder CreateBuilder()
        {
            var asmName = new AssemblyName("RomanticWeb.Dynamic");
            return AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
        }
    }
}