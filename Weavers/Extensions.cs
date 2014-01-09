using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Weavers
{
    public static class Extensions
    {
        internal static bool HasEntityIdAncestor(this TypeDefinition childType)
        {
            var baseType=childType.BaseType;

            while (baseType!=null)
            {
                if (baseType.FullName=="RomanticWeb.Entities.EntityId")
                {
                    return true;
                }

                baseType=baseType.Resolve().BaseType;
            }

            return false;
        }

        internal static bool HasNoTypeConverter(this TypeDefinition type)
        {
            return type.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.ComponentModel.TypeConverterAttribute");
        }

        internal static bool HasRequiredConstructor(this TypeDefinition type)
        {
            return (from constructor in type.GetConstructors()
                    where constructor.Parameters.Count == 1
                    let parameter=constructor.Parameters.Single()
                    where parameter.ParameterType.FullName==typeof(Uri).FullName
                    select constructor).Any();
        }
    }
}