using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mono.Cecil;

namespace RomanticWeb.Fody
{
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1512:SingleLineCommentsMustNotBeFollowedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]
    public partial class ModuleWeaver
    {
// ReSharper disable InconsistentNaming
        private AssemblyDefinition RomanticWebAssembly;
// ReSharper restore InconsistentNaming

        private void LoadDependencies()
        {
            RomanticWebAssembly=LoadAssemblyReference("RomanticWeb");
        }

        private AssemblyDefinition LoadAssemblyReference(string assemblyFullName)
        {
            var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == assemblyFullName);

            if (existingReference != null)
            {
                return AssemblyResolver.Resolve(existingReference);
            }

            var reference = AssemblyResolver.Resolve(assemblyFullName);
            if (reference != null)
            {
                return reference;
            }

            throw new Exception(string.Format("Could not resolve a reference to {0}.", assemblyFullName));
        }
    }
}