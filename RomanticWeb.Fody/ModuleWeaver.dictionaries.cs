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

        private IEnumerable<DictionaryPropertyMapping> DictionaryPropertiesMappedWithAttributes
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
            _factory=new DictionaryPropertyMappingFactory(ModuleDefinition,Imports);

            foreach (var dictionaryProperty in DictionaryPropertiesMappedWithAttributes.ToList())
            {
                CreateDictionaryEntryType(dictionaryProperty);
                CreateDictionaryOwnerType(dictionaryProperty);

                ModuleDefinition.Types.Add(dictionaryProperty.OwnerType);
                ModuleDefinition.Types.Add(dictionaryProperty.EntryType);

                var ownerMapping=CreateOwnerMapping(dictionaryProperty);
                var entryMapping=CreateEntryMapping(dictionaryProperty);

                ownerMapping.Methods.Add(CreateDictionaryEntriesMappingOverride(dictionaryProperty));
                entryMapping.Methods.Add(CreateEntryKeyMappingOverride());
                entryMapping.Methods.Add(CreateEntryValueMappingOverride());

                ModuleDefinition.Types.Add(ownerMapping);
                ModuleDefinition.Types.Add(entryMapping);
            }
        }

        private TypeDefinition CreateOwnerMapping(DictionaryPropertyMapping propertyMapping)
        {
            TypeReference[] genericArgs = propertyMapping.OwnerType.JoinWith(propertyMapping.EntryType.JoinWith(propertyMapping.GenericArguments));
            TypeReference baseType=ModuleDefinition.Import(Imports.DictionaryOwnerMapTypeRef.MakeGenericInstanceType(genericArgs));
            var mappingType = CreateMappingType(propertyMapping.OwnerType, baseType);
            mappingType.Methods.Add(CreateDefaultConstructor(mappingType,genericArgs));
            return mappingType;
        }

        private TypeDefinition CreateEntryMapping(DictionaryPropertyMapping dictionaryMapping)
        {
            TypeReference[] genericArgs=dictionaryMapping.EntryType.JoinWith(dictionaryMapping.GenericArguments);
            TypeReference baseType=ModuleDefinition.Import(Imports.DictionaryEntryMapTypeRef.MakeGenericInstanceType(genericArgs));
            var mappingType = CreateMappingType(dictionaryMapping.EntryType, baseType);
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

        private MethodDefinition CreateDictionaryEntriesMappingOverride(DictionaryPropertyMapping mapping)
        {
            var @override=new MethodDefinition("SetupEntriesCollection",OverrideMethodAttributes,ModuleDefinition.TypeSystem.Void);

            @override.Parameters.Add(new ParameterDefinition(ModuleDefinition.Import(Imports.TermPartType.MakeGenericInstanceType(Imports.CollectionMapTypeRef))));

            var ilProcessor=@override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            mapping.InjectDictionaryEntriesTermMappingCode(ilProcessor);
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private MethodDefinition CreateEntryKeyMappingOverride()
        {
            var @override = new MethodDefinition("SetupKeyProperty", OverrideMethodAttributes, ModuleDefinition.TypeSystem.Void);
            @override.Parameters.Add(new ParameterDefinition(Imports.PropertyMapTermPartTypeRef));

            var ilProcessor = @override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld,Imports.PredicateFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, ModuleDefinition.Import(Imports.TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(Imports.PropertyMapTypeRef))));
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private MethodDefinition CreateEntryValueMappingOverride()
        {
            var @override = new MethodDefinition("SetupValueProperty", OverrideMethodAttributes, ModuleDefinition.TypeSystem.Void);
            @override.Parameters.Add(new ParameterDefinition(Imports.PropertyMapTermPartTypeRef));

            var ilProcessor = @override.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Nop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, Imports.ObjectFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, ModuleDefinition.Import(Imports.TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(Imports.PropertyMapTypeRef))));
            ilProcessor.Append(Instruction.Create(OpCodes.Pop));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return @override;
        }

        private void CreateDictionaryOwnerType(DictionaryPropertyMapping dictionaryProperty)
        {
            var declaringType=dictionaryProperty.Property.DeclaringType;
            var dictOwnerName=string.Format("{0}_{1}_Owner",declaringType.Name,dictionaryProperty.Property.Name);
            var dictionaryOwnerType=new TypeDefinition(declaringType.Namespace,dictOwnerName,declaringType.Attributes);

            var genericArgs=dictionaryProperty.EntryType.JoinWith(dictionaryProperty.GenericArguments);
            var baseType=Imports.DictionaryOwnerTypeRef.MakeGenericInstanceType(genericArgs);
            dictionaryOwnerType.Interfaces.Add(baseType);

            dictionaryOwnerType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));

            dictionaryProperty.OwnerType=dictionaryOwnerType;
        }

        private void CreateDictionaryEntryType(DictionaryPropertyMapping dictionaryProperty)
        {
            var declaringType=dictionaryProperty.Property.DeclaringType;
            var dictEntryName=string.Format("{0}_{1}_Entry",declaringType.Name,dictionaryProperty.Property.Name);
            var entryType=new TypeDefinition(declaringType.Namespace,dictEntryName,declaringType.Attributes);

            var baseType=Imports.DictionaryEntryTypeRef.MakeGenericInstanceType(dictionaryProperty.GenericArguments);
            entryType.Interfaces.Add(baseType);

            entryType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));

            dictionaryProperty.EntryType=entryType;
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
