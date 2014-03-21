using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using RomanticWeb.Fody.Dictionaries;

namespace RomanticWeb.Fody
{
    public partial class ModuleWeaver
    {
        private IEnumerable<DictionaryMappingMeta> DictionaryPropertiesInEntities
        {
            get
            {
                return from typeDefinition in ModuleDefinition.Types
                       where typeDefinition.IsInterface
                       where typeDefinition.Implements(Imports.EntityTypeRef)
                       from property in typeDefinition.Properties
                       let returnType=property.PropertyType.Resolve()
                       where returnType!=null
                       where property.PropertyType.Resolve().Implements(Imports.DictionaryTypeRef)
                       select new DictionaryMappingMeta(property);
            }
        }

        private void AddDictionaryEntityTypes()
        {
            foreach (var dictionaryProperty in DictionaryPropertiesInEntities.ToList())
            {
                CreateDictionaryEntryType(dictionaryProperty);
                CreateDictionaryOwnerType(dictionaryProperty);

                ModuleDefinition.Types.Add(dictionaryProperty.OwnerType);
                ModuleDefinition.Types.Add(dictionaryProperty.EntryType);
            }
        }

        private void CreateDictionaryOwnerType(DictionaryMappingMeta dictionary)
        {
            var declaringType=dictionary.Property.DeclaringType;
            var names=new CecilDictionaryEntityNames(dictionary.Property);
            var dictionaryOwnerType=new TypeDefinition(names.Namespace,names.OwnerTypeName,declaringType.Attributes);

            var genericArgs=dictionary.EntryType.JoinWith(dictionary.GenericArguments);
            var baseType=Imports.DictionaryOwnerTypeRef.MakeGenericInstanceType(genericArgs);
            dictionaryOwnerType.Interfaces.Add(baseType);

            dictionaryOwnerType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));

            dictionary.OwnerType=dictionaryOwnerType;
        }

        private void CreateDictionaryEntryType(DictionaryMappingMeta dictionary)
        {
            var declaringType = dictionary.Property.DeclaringType;
            var names=new CecilDictionaryEntityNames(dictionary.Property);
            var entryType=new TypeDefinition(names.Namespace,names.EntryTypeName,declaringType.Attributes);

            var baseType=Imports.DictionaryEntryTypeRef.MakeGenericInstanceType(dictionary.GenericArguments);
            entryType.Interfaces.Add(baseType);

            entryType.CustomAttributes.Add(new CustomAttribute(Imports.CompilerGeneratedAttributeCtorRef));

            dictionary.EntryType=entryType;
        }
    }
}
