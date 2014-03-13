using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RomanticWeb.Fody.Dictionaries
{
    internal class DictionaryPropertyMapping
    {
        internal DictionaryPropertyMapping(PropertyDefinition property,Action<ILProcessor> injectDictionaryEntriesTermMappingCode)
        {
            InjectDictionaryEntriesTermMappingCode=injectDictionaryEntriesTermMappingCode;
            Property=property;
            GenericArguments=((GenericInstanceType)property.PropertyType).GenericArguments.ToArray();
        }

        public TypeReference[] GenericArguments { get; private set; }

        public PropertyDefinition Property { get; private set; }

        public TypeDefinition EntryType { get; set; }

        public TypeDefinition OwnerType { get; set; }

        public Action<ILProcessor> InjectDictionaryEntriesTermMappingCode { get; private set; } 
    }
}