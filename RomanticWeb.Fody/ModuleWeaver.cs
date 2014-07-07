using System;
using System.Linq;
using Mono.Cecil;

namespace RomanticWeb.Fody
{
    public partial class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public IAssemblyResolver AssemblyResolver { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogError { get; set; }

        private WeaverImports Imports { get; set; }

        private WeaverReferences References { get; set; }

        public void Execute()
        {
            AssemblyDefinition ormAssembly;
            if (ModuleIsRomanticWeb() || ModuleHasNoRomanticWebReference(out ormAssembly))
            {
                // skip RomanticWeb assemblies and those that don't reference RomanticWeb
                return;
            }

            References = new WeaverReferences(this);
            Imports = new WeaverImports(this, References);

            AddTypeConverters();
            AddDictionaryEntityTypes();
        }

        private bool ModuleHasNoRomanticWebReference(out AssemblyDefinition ormAssembly)
        {
            var reference = ModuleDefinition.AssemblyReferences.FirstOrDefault(refe => refe.FullName.StartsWith("RomanticWeb,"));

            if (reference != null)
            {
                ormAssembly = ModuleDefinition.AssemblyResolver.Resolve(reference);
                return false;
            }

            ormAssembly = null;
            return true;
        }

        private bool ModuleIsRomanticWeb()
        {
            return ModuleDefinition.FullyQualifiedName.StartsWith("RomanticWeb");
        }
    }
}
