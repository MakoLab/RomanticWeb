using Mono.Cecil;

namespace RomanticWeb.Fody.Dictionaries
{
    internal class DictionaryMappingMeta
    {
        internal DictionaryMappingMeta(PropertyDefinition property)
        {
            Property = property;
            GenericArguments = ((GenericInstanceType)property.PropertyType).GenericArguments.ToArray();
        }

        public TypeReference[] GenericArguments { get; private set; }

        public PropertyDefinition Property { get; private set; }

        public TypeDefinition EntryType { get; set; }

        public TypeDefinition OwnerType { get; set; }
    }
}