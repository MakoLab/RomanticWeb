using Mono.Cecil;
using RomanticWeb.Dynamic;

namespace RomanticWeb.Fody.Dictionaries
{
    internal class CecilDictionaryEntityNames:DictionaryEntityNames
    {
        public CecilDictionaryEntityNames(PropertyReference property)
            :base(
                property.DeclaringType.Namespace,
                property.DeclaringType.Name,
                property.Name,
                property.DeclaringType.Module.Assembly.Name.Name)
        {
        }
    }
}