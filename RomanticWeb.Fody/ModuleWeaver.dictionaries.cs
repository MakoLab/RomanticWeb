using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace RomanticWeb.Fody
{
    public partial class ModuleWeaver
    {
        private const MethodAttributes ConstructorAttributes=MethodAttributes.Public|MethodAttributes.HideBySig|MethodAttributes.RTSpecialName|MethodAttributes.SpecialName;
        private const TypeAttributes MapTypeAttributes=TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
        private const MethodAttributes OverrideMethodAttributes=MethodAttributes.Public|MethodAttributes.Family|MethodAttributes.Virtual|MethodAttributes.HideBySig;

        private void AddDictionaryEntityTypesAndMappings()
        {
            foreach (var dictionaryProperty in GetEntityTypesWithDictionary().ToList())
            {
                var dictionaryGenericArguments=((GenericInstanceType)dictionaryProperty.PropertyType).GenericArguments.ToArray();

                var entryType=CreateDictionaryEntryType(dictionaryProperty,dictionaryGenericArguments);
                var ownerType=CreateDictionaryOwnerType(dictionaryProperty,entryType,dictionaryGenericArguments);

                ModuleDefinition.Types.Add(ownerType);
                ModuleDefinition.Types.Add(entryType);

                var ownerMapping=CreateOwnerMapping(ownerType,entryType,dictionaryGenericArguments);
                var entryMapping=CreateEntryMapping(entryType,dictionaryGenericArguments);

                ownerMapping.Methods.Add(CreateDictionaryEntriesMappingOverride());
                entryMapping.Methods.Add(CreateEntryKeyMappingOverride());
                entryMapping.Methods.Add(CreateEntryValueMappingOverride());

                ModuleDefinition.Types.Add(ownerMapping);
                ModuleDefinition.Types.Add(entryMapping);
            }
        }

        private TypeDefinition CreateOwnerMapping(TypeDefinition ownerType,TypeDefinition entryType,IEnumerable<TypeReference> dictionaryGenericArguments)
        {
            TypeReference[] genericArgs=ownerType.JoinWith(entryType.JoinWith(dictionaryGenericArguments));
            TypeReference baseType=ModuleDefinition.Import(DictionaryOwnerMapTypeRef.MakeGenericInstanceType(genericArgs));
            var mappingType=CreateMappingType(ownerType,baseType);
            mappingType.Methods.Add(CreateDefaultConstructor(mappingType,genericArgs));
            return mappingType;
        }

        private TypeDefinition CreateEntryMapping(TypeDefinition entryType,IEnumerable<TypeReference> dictionaryGenericArguments)
        {
            TypeReference[] genericArgs=entryType.JoinWith(dictionaryGenericArguments);
            TypeReference baseType=ModuleDefinition.Import(DictionaryEntryMapTypeRef.MakeGenericInstanceType(genericArgs));
            var mappingType=CreateMappingType(entryType,baseType);
            mappingType.Methods.Add(CreateDefaultConstructor(mappingType,genericArgs));
            return mappingType;
        }

        private TypeDefinition CreateMappingType(TypeDefinition mappedType, TypeReference baseType)
        {
            string mappingName=string.Format("{0}Map",mappedType.Name.TrimStart('I'));
            var mappingType=new TypeDefinition(mappedType.Namespace,mappingName,MapTypeAttributes) { BaseType=baseType };
            mappingType.CustomAttributes.Add(new CustomAttribute(CompilerGeneratedAttributeCtorRef));
            return mappingType;
        }

        private MethodDefinition CreateDictionaryEntriesMappingOverride()
        {
            var @override=new MethodDefinition("SetupEntriesCollection",OverrideMethodAttributes,ModuleDefinition.TypeSystem.Void);

            @override.Parameters.Add(new ParameterDefinition(ModuleDefinition.Import(TermPartType.MakeGenericInstanceType(CollectionMapTypeRef))));

            var ilProcessor=@override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldstr,"magi"));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldstr,"setting"));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt,ModuleDefinition.Import(TermPartIsQNameMethodRef.Resolve().MakeHostInstanceGeneric(CollectionMapTypeRef))));
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private MethodDefinition CreateEntryKeyMappingOverride()
        {
            var @override = new MethodDefinition("SetupKeyProperty", OverrideMethodAttributes, ModuleDefinition.TypeSystem.Void);
            @override.Parameters.Add(new ParameterDefinition(PropertyMapTermPartTypeRef));

            var ilProcessor = @override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld,PredicateFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, ModuleDefinition.Import(TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(PropertyMapTypeRef))));
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private MethodDefinition CreateEntryValueMappingOverride()
        {
            var @override = new MethodDefinition("SetupValueProperty", OverrideMethodAttributes, ModuleDefinition.TypeSystem.Void);
            @override.Parameters.Add(new ParameterDefinition(PropertyMapTermPartTypeRef));

            var ilProcessor = @override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, ObjectFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, ModuleDefinition.Import(TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(PropertyMapTypeRef))));
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private TypeDefinition CreateDictionaryOwnerType(PropertyDefinition dictionaryProperty,TypeDefinition entryType,IEnumerable<TypeReference> dictionaryGenericArguments)
        {
            var declaringType=dictionaryProperty.DeclaringType;
            var dictOwnerName=string.Format("{0}_{1}_Owner",declaringType.Name,dictionaryProperty.Name);
            var dictionaryOwnerType=new TypeDefinition(declaringType.Namespace,dictOwnerName,declaringType.Attributes);

            var genericArgs=entryType.JoinWith(dictionaryGenericArguments);
            var baseType=DictionaryOwnerTypeRef.MakeGenericInstanceType(genericArgs);
            dictionaryOwnerType.Interfaces.Add(baseType);

            dictionaryOwnerType.CustomAttributes.Add(new CustomAttribute(CompilerGeneratedAttributeCtorRef));

            return dictionaryOwnerType;
        }

        private TypeDefinition CreateDictionaryEntryType(PropertyDefinition dictionaryProperty,TypeReference[] dictionaryGenericArguments)
        {
            var declaringType=dictionaryProperty.DeclaringType;
            var dictEntryName=string.Format("{0}_{1}_Entry",declaringType.Name,dictionaryProperty.Name);
            var entryType=new TypeDefinition(declaringType.Namespace,dictEntryName,declaringType.Attributes);

            var baseType=DictionaryEntryTypeRef.MakeGenericInstanceType(dictionaryGenericArguments);
            entryType.Interfaces.Add(baseType);

            entryType.CustomAttributes.Add(new CustomAttribute(CompilerGeneratedAttributeCtorRef));

            return entryType;
        }

        private MethodDefinition CreateDefaultConstructor(TypeDefinition type,IEnumerable<TypeReference> dictionaryGenericArguments)
        {
            var baseConstructor=ModuleDefinition.Import(type.BaseType.Resolve().GetConstructors().Single().MakeHostInstanceGeneric(dictionaryGenericArguments.ToArray()));
            var ctor = new MethodDefinition(".ctor", ConstructorAttributes, ModuleDefinition.TypeSystem.Void);
            ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseConstructor));
            ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            return ctor;
        }

        private IEnumerable<PropertyDefinition> GetEntityTypesWithDictionary()
        {
            return from typeDefinition in ModuleDefinition.Types
                   from property in typeDefinition.Properties
                   where HasDictionaryAttribute(property)
                   select property;
        }

        private bool HasDictionaryAttribute(PropertyDefinition propDefinition)
        {
            return propDefinition.CustomAttributes.Any(attr => attr.AttributeType.Name=="DictionaryAttribute");
        }
    }
}
