using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace RomanticWeb.Fody
{
    public partial class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public IAssemblyResolver AssemblyResolver { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogError { get; set; }

        public void Execute()
        {
            AssemblyDefinition ormAssembly;
            if (ModuleIsRomanticWeb() || ModuleHasNoRomanticWebReference(out ormAssembly))
            {
                // skip RomanticWeb assemblies and those that don't reference RomanticWeb
                return;
            }
            
            LoadDependencies();
            ImportTypes();

            AddTypeConverters();
            AddDictionaryEntityTypesAndMappings();
        }

        private bool ModuleHasNoRomanticWebReference(out AssemblyDefinition ormAssembly)
        {
            var reference=ModuleDefinition.AssemblyReferences.FirstOrDefault(refe => refe.FullName.StartsWith("RomanticWeb,"));

            if (reference!=null)
            {
                ormAssembly=ModuleDefinition.AssemblyResolver.Resolve(reference);
                return false;
            }

            ormAssembly=null;
            return true;
        }

        private bool ModuleIsRomanticWeb()
        {
            return ModuleDefinition.FullyQualifiedName.StartsWith("RomanticWeb");
        }
    }
}
