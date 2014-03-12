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
        private void AddTypeConverters()
        {
            foreach (var entityIdType in GetEntityIdImplementationsWithoutConverter().ToList())
            {
                if (!entityIdType.HasRequiredConstructor())
                {
                    LogError(string.Format("Type {0} must have a constructor, which takes a single System.Uri parameter", entityIdType));
                    continue;
                }

                AddTypeConverter(entityIdType);
            }
        }

        private void AddTypeConverter(TypeDefinition entityIdType)
        {
            LogInfo(string.Format("Adding type converter to {0}", entityIdType.Name));

            // get [TypeConverter] attribute constructor
            var constructor = ModuleDefinition.Import(typeof(TypeConverterAttribute).GetConstructor(new[] { typeof(Type) }));

            // get EntityIdTypeConverter<> base type
            var convererType = ModuleDefinition.Import(RomanticWebAssembly.MainModule.GetType("RomanticWeb.ComponentModel.EntityIdTypeConverter`1"));

            // make generic type EntityIdTypeConverter<entityIdType>
            var genericConverter = convererType.MakeGenericInstanceType(entityIdType);

            var attribute = new CustomAttribute(constructor);
            attribute.ConstructorArguments.Add(new CustomAttributeArgument(SystemTypeRef,genericConverter));
            entityIdType.CustomAttributes.Add(attribute);
        }

        private IEnumerable<TypeDefinition> GetEntityIdImplementationsWithoutConverter()
        {
            return from typeDefinition in ModuleDefinition.Types
                   where typeDefinition.HasEntityIdAncestor() || typeDefinition.HasNoTypeConverter()
                   select typeDefinition;
        }
    }
}
