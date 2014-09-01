using System;
using System.Reflection;
using System.Reflection.Emit;

namespace RomanticWeb.Dynamic
{
    internal class EmitHelper
    {
        private const string DynamicMappingModuleName = "DynamicDictionaryMappings";
        private static readonly object Locker = new object();
        private readonly Lazy<AssemblyBuilder> _asmBuilder;
        private readonly Guid _assemblyGuid = Guid.NewGuid();

        public EmitHelper()
        {
            _asmBuilder = new Lazy<AssemblyBuilder>(CreateBuilder);
        }

        public ModuleBuilder GetDynamicModule()
        {
            lock (Locker)
            {
                var assemblyBuilder = GetBuilder();
                return assemblyBuilder.GetDynamicModule(DynamicMappingModuleName) ?? assemblyBuilder.DefineDynamicModule(DynamicMappingModuleName);
            }
        }

        public AssemblyBuilder GetBuilder()
        {
            return _asmBuilder.Value;
        }

        private AssemblyBuilder CreateBuilder()
        {
            var asmName = new AssemblyName("RomanticWeb.Dynamic_" + _assemblyGuid.ToString("N"));
            return AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
        }
    }
}