using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace RomanticWeb.Fody
{
    public static class Extensions
    {
        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }

            foreach (var genericParameter in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));
            }

            return reference;
        }

        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes)
        {
            var firstOrDefault = typeDefinition.Methods
                                               .FirstOrDefault(x =>
                                                               !x.HasGenericParameters &&
                                                               x.Name == method &&
                                                               IsMatch(x, paramTypes));
            if (firstOrDefault == null)
            {
                var parameterNames = string.Join(", ", paramTypes);
                throw new WeavingException(string.Format("Expected to find method '{0}({1})' on type '{2}'.", method, parameterNames, typeDefinition.FullName));
            }

            return firstOrDefault;
        }

        internal static bool HasEntityIdAncestor(this TypeDefinition childType)
        {
            return childType.HasAncestorOfType("RomanticWeb.Entities.EntityId");
        }

        internal static bool HasNancyModuleAncestor(this TypeDefinition childType)
        {
            return childType.HasAncestorOfType("Nancy.NancyModule");
        }

        internal static bool HasAncestorOfType(this TypeDefinition childType, string typeName)
        {
            var baseType=childType.BaseType;

            while (baseType!=null)
            {
                if (baseType.FullName==typeName)
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
        
        internal static TypeReference[] JoinWith(this TypeReference reference, IEnumerable<TypeReference> otherRefs)
        {
            var typeReferences = otherRefs as TypeReference[] ?? otherRefs.ToArray();
            var genericArgs = new TypeReference[typeReferences.Count() + 1];
            genericArgs[0] = reference;
            typeReferences.CopyTo(genericArgs, 1);
            return genericArgs;
        }


        private static bool IsMatch(this MethodReference methodReference, params string[] paramTypes)
        {
            if (methodReference.Parameters.Count != paramTypes.Length)
            {
                return false;
            }

            for (var index = 0; index < methodReference.Parameters.Count; index++)
            {
                var parameterDefinition = methodReference.Parameters[index];
                var paramType = paramTypes[index];
                if (parameterDefinition.ParameterType.Name != paramType)
                {
                    return false;
                }
            }

            return true;
        }
    }
}