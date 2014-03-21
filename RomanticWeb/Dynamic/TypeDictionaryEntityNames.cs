using System.Reflection;

namespace RomanticWeb.Dynamic
{
    internal class TypeDictionaryEntityNames:DictionaryEntityNames
    {
        public TypeDictionaryEntityNames(PropertyInfo property)
            :base(
                property.DeclaringType.Namespace,
                property.DeclaringType.Name,
                property.Name,
                property.DeclaringType.Assembly.FullName)
        {
        }
    }
}