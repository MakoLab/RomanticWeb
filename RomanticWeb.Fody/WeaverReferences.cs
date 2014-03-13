using System;
using System.Linq;
using Mono.Cecil;

namespace RomanticWeb.Fody
{
    internal class WeaverReferences
    {
        private readonly ModuleWeaver _weaver;

        public WeaverReferences(ModuleWeaver weaver)
        {
            _weaver=weaver;
            Orm=LoadAssemblyReference("RomanticWeb");
        }

        public AssemblyDefinition Orm { get; private set; }

        private AssemblyDefinition LoadAssemblyReference(string assemblyFullName)
        {
            var existingReference = _weaver.ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == assemblyFullName);

            if (existingReference != null)
            {
                return _weaver.AssemblyResolver.Resolve(existingReference);
            }

            var reference = _weaver.AssemblyResolver.Resolve(assemblyFullName);
            if (reference != null)
            {
                return reference;
            }

            throw new Exception(string.Format("Could not resolve a reference to {0}.", assemblyFullName));
        }
    }
}