using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace RomanticWeb.Fody
{
    public class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }

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

            var systemTypeRef = ModuleDefinition.Import(typeof(Type));

            AddTypeConverters(ormAssembly, systemTypeRef);
        }

        private void AddTypeConverters(AssemblyDefinition ormAssembly, TypeReference systemTypeRef)
        {
            foreach (var entityIdType in GetEntityIdImplementationsWithoutConverter().ToList())
            {
                if (!entityIdType.HasRequiredConstructor())
                {
                    LogError(string.Format("Type {0} must have a constructor, which takes a single System.Uri parameter", entityIdType));
                    continue;
                }

                AddTypeConverter(entityIdType, ormAssembly, systemTypeRef);
            }
        }

        private void AddTypeConverter(TypeDefinition entityIdType, AssemblyDefinition ormAssembly, TypeReference systemTypeRef)
        {
            LogInfo(string.Format("Adding type converter to {0}", entityIdType.Name));

            // get [TypeConverter] attribute constructor
            var constructor = ModuleDefinition.Import(typeof(TypeConverterAttribute).GetConstructor(new[] { typeof(Type) }));

            // get EntityIdTypeConverter<> base type
            var convererType = ModuleDefinition.Import(ormAssembly.MainModule.GetType("RomanticWeb.ComponentModel.EntityIdTypeConverter`1"));

            // make generic type EntityIdTypeConverter<entityIdType>
            var genericConverter = convererType.MakeGenericInstanceType(entityIdType);

            var attribute = new CustomAttribute(constructor);
            attribute.ConstructorArguments.Add(new CustomAttributeArgument(systemTypeRef, genericConverter));
            entityIdType.CustomAttributes.Add(attribute);
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

        private IEnumerable<TypeDefinition> GetEntityIdImplementationsWithoutConverter()
        {
            return from typeDefinition in ModuleDefinition.Types
                   where typeDefinition.HasEntityIdAncestor() || typeDefinition.HasNoTypeConverter()
                   select typeDefinition;
        }
    }
}
