using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RomanticWeb.Fody.Dictionaries
{
    internal class DictionaryMappingMeta
    {
        internal DictionaryMappingMeta(PropertyDefinition property,Action<ILProcessor> injectDictionaryEntriesTermMappingCode,Action<ILProcessor> injectKeyMappingCode)
        {
            InjectDictionaryEntriesTermMappingCode=injectDictionaryEntriesTermMappingCode;
            InjectKeyMappingCode=injectKeyMappingCode;
            Property=property;
            GenericArguments=((GenericInstanceType)property.PropertyType).GenericArguments.ToArray();
        }

        public TypeReference[] GenericArguments { get; private set; }

        public PropertyDefinition Property { get; private set; }

        public TypeDefinition EntryType { get; set; }

        public TypeDefinition OwnerType { get; set; }

        public Action<ILProcessor> InjectDictionaryEntriesTermMappingCode { get; private set; }

        public Action<ILProcessor> InjectKeyMappingCode { get; private set; }
    }
}