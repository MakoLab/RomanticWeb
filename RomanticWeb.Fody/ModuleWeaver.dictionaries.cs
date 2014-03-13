using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using RomanticWeb.Fody.Dictionaries;

namespace RomanticWeb.Fody
{
    public partial class ModuleWeaver
    {
        private const MethodAttributes ConstructorAttributes=MethodAttributes.Public|MethodAttributes.HideBySig|MethodAttributes.RTSpecialName|MethodAttributes.SpecialName;
        private const TypeAttributes MapTypeAttributes=TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
        private const MethodAttributes OverrideMethodAttributes=MethodAttributes.Public|MethodAttributes.Family|MethodAttributes.Virtual|MethodAttributes.HideBySig;

        private DictionaryPropertyMappingFactory _factory;

        private IEnumerable<DictionaryMappingMeta> DictionaryPropertiesMappedWithAttributes
        {
            get
            {
                return from typeDefinition in ModuleDefinition.Types
                       from property in typeDefinition.Properties
                       where HasDictionaryAttribute(property)
                       select _factory.CreateFromAttributeMapping(property);
            }
        }

        private void AddDictionaryEntityTypesAndMappings()
        {
            _factory=new DictionaryPropertyMappingFactory(Imports);

            foreach (var dictionaryProperty in DictionaryPropertiesMappedWithAttributes.ToList())
            {
                CreateDictionaryEntryType(dictionaryProperty);
                CreateDictionaryOwnerType(dictionaryProperty);

                ModuleDefinition.Types.Add(dictionaryProperty.OwnerType);
                ModuleDefinition.Types.Add(dictionaryProperty.EntryType);

                var ownerMapping=CreateOwnerMapping(dictionaryProperty);
                var entryMapping=CreateEntryMapping(dictionaryProperty);

                ownerMapping.Methods.Add(CreateDictionaryEntriesMappingOverride(dictionaryProperty));
                entryMapping.Methods.Add(CreateEntryKeyMappingOverride(dictionaryProperty));
                entryMapping.Methods.Add(CreateEntryValueMappingOverride(dictionaryProperty));

                ModuleDefinition.Types.Add(ownerMapping);
                ModuleDefinition.Types.Add(entryMapping);
            }
        }

        private TypeDefinition CreateOwnerMapping(DictionaryMappingMeta mappingMeta)
        {
            TypeReference[] genericArgs = mappingMeta.OwnerType.JoinWith(mappingMeta.EntryType.JoinWith(mappingMeta.GenericArguments));
            TypeReference baseType=ModuleDefinition.Import(Imports.DictionaryOwnerMapTypeRef.MakeGenericInstanceType(genericArgs));
            var mappingType = CreateMappingType(mappingMeta.OwnerType, baseType);
            mappingType.Methods.Add(CreateDefaultConstructor(mappingType,genericArgs));
            return mappingType;
        }

        private TypeDefinition CreateEntryMapping(DictionaryMappingMeta dictionaryMappingMeta)
        {
            TypeReference[] genericArgs=dictionaryMappingMeta.EntryType.JoinWith(dictionaryMappingMeta.GenericArguments);
            TypeReference baseType=ModuleDefinition.Import(Imports.DictionaryEntryMapTypeRef.MakeGenericInstanceType(genericArgs));
            var mappingType = CreateMappingType(dictionaryMappingMeta.EntryType, baseType);
            mappingType.Methods.Add(CreateDefaultConstructor(mappingType,genericArgs));
            return mappingType;
        }

        private TypeDefinition CreateMappingType(TypeDefinition mappedType,TypeReference baseType)
        {
            string mappingName=string.Format("{0}Map",mappedType.Name.TrimStart('I'));
            var mappingType=new TypeDefinition(mappedType.Namespace,mappingName,MapTypeAttributes) { BaseType=baseType };
            mappingType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));
            return mappingType;
        }

        private MethodDefinition CreateDictionaryEntriesMappingOverride(DictionaryMappingMeta mappingMeta)
        {
            var @override=new MethodDefinition("SetupEntriesCollection",OverrideMethodAttributes,ModuleDefinition.TypeSystem.Void);

            @override.Parameters.Add(new ParameterDefinition(ModuleDefinition.Import(Imports.TermPartType.MakeGenericInstanceType(Imports.CollectionMapTypeRef))));

            var ilProcessor=@override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            mappingMeta.InjectDictionaryEntriesTermMappingCode(ilProcessor);
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private MethodDefinition CreateEntryKeyMappingOverride(DictionaryMappingMeta mappingMeta)
        {
            var @override=new MethodDefinition("SetupKeyProperty",OverrideMethodAttributes,ModuleDefinition.TypeSystem.Void);
            @override.Parameters.Add(new ParameterDefinition(Imports.PropertyMapTermPartTypeRef));

            var ilProcessor = @override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            mappingMeta.InjectKeyMappingCode(ilProcessor);
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private MethodDefinition CreateEntryValueMappingOverride(DictionaryMappingMeta dictionaryProperty)
        {
            var @override = new MethodDefinition("SetupValueProperty", OverrideMethodAttributes, ModuleDefinition.TypeSystem.Void);
            @override.Parameters.Add(new ParameterDefinition(Imports.PropertyMapTermPartTypeRef));

            var ilProcessor = @override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            dictionaryProperty.InjectValueMappingCode(ilProcessor);
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private void CreateDictionaryOwnerType(DictionaryMappingMeta dictionary)
        {
            var declaringType=dictionary.Property.DeclaringType;
            var dictOwnerName=string.Format("{0}_{1}_Owner",declaringType.Name,dictionary.Property.Name);
            var dictionaryOwnerType=new TypeDefinition(declaringType.Namespace,dictOwnerName,declaringType.Attributes);

            var genericArgs=dictionary.EntryType.JoinWith(dictionary.GenericArguments);
            var baseType=Imports.DictionaryOwnerTypeRef.MakeGenericInstanceType(genericArgs);
            dictionaryOwnerType.Interfaces.Add(baseType);

            dictionaryOwnerType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));

            dictionary.OwnerType=dictionaryOwnerType;
        }

        private void CreateDictionaryEntryType(DictionaryMappingMeta dictionary)
        {
            var declaringType=dictionary.Property.DeclaringType;
            var dictEntryName=string.Format("{0}_{1}_Entry",declaringType.Name,dictionary.Property.Name);
            var entryType=new TypeDefinition(declaringType.Namespace,dictEntryName,declaringType.Attributes);

            var baseType=Imports.DictionaryEntryTypeRef.MakeGenericInstanceType(dictionary.GenericArguments);
            entryType.Interfaces.Add(baseType);

            entryType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));

            dictionary.EntryType=entryType;
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

        private bool HasDictionaryAttribute(PropertyDefinition propDefinition)
        {
            return propDefinition.CustomAttributes.Any(attr => attr.AttributeType.Name=="DictionaryAttribute");
        }
    }
}
