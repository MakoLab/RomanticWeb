using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace RomanticWeb.Fody
{
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1512:SingleLineCommentsMustNotBeFollowedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]
    public partial class ModuleWeaver
    {
// ReSharper disable InconsistentNaming
        private TypeReference SystemTypeRef;
        private TypeReference DictionaryOwnerTypeRef;
        private TypeReference DictionaryEntryTypeRef;
        private TypeReference CollectionMapTypeRef;
        private TypeReference TermPartType;
        private TypeReference DictionaryOwnerMapTypeRef;
        private TypeReference DictionaryEntryMapTypeRef;
        private TypeReference PropertyMapTermPartTypeRef;
        private TypeReference PropertyMapTypeRef;

        private MethodReference CompilerGeneratedAttributeCtorRef;
        private MethodReference TermPartIsQNameMethodRef;
        private MethodReference TermPartIsUriMethodRef;

        private FieldReference ObjectFieldRef;
        private FieldReference PredicateFieldRef;
// ReSharper restore InconsistentNaming

        private void ImportTypes()
        {
            SystemTypeRef=ModuleDefinition.Import(typeof(Type));
            var dictionaryOwnerType=RomanticWebAssembly.MainModule.Types.Single(t => t.Name=="IDictionaryOwner`3");
            var dictionaryEntryType=RomanticWebAssembly.MainModule.Types.Single(t => t.Name=="IKeyValuePair`2");
            var dictOwnerMapType = RomanticWebAssembly.MainModule.Types.Single(t => t.Name == "DictionaryOwnerMap`4");
            var dictEntryMapType = RomanticWebAssembly.MainModule.Types.Single(t => t.Name == "DictionaryEntryMap`3");
            var termPartType=RomanticWebAssembly.MainModule.Types.Single(t => t.Name=="TermPart`1");
            var propertyMapType=RomanticWebAssembly.MainModule.Types.Single(t => t.Name=="PropertyMap");
            var rdfVocabularyType=RomanticWebAssembly.MainModule.Types.Single(t => t.FullName=="RomanticWeb.Vocabularies.Rdf");
            var compilerGeneratedAttributeType= ModuleDefinition.Import(typeof(CompilerGeneratedAttribute)).Resolve();

            DictionaryOwnerTypeRef=ModuleDefinition.Import(dictionaryOwnerType);
            DictionaryEntryTypeRef=ModuleDefinition.Import(dictionaryEntryType);
            DictionaryOwnerMapTypeRef=ModuleDefinition.Import(dictOwnerMapType);
            DictionaryEntryMapTypeRef=ModuleDefinition.Import(dictEntryMapType);
            TermPartType=ModuleDefinition.Import(termPartType);
            PropertyMapTermPartTypeRef=ModuleDefinition.Import(termPartType.MakeGenericInstanceType(propertyMapType));
            CollectionMapTypeRef = ModuleDefinition.Import(RomanticWebAssembly.MainModule.Types.Single(t => t.Name == "CollectionMap"));
            TermPartIsQNameMethodRef=ModuleDefinition.Import(termPartType.FindMethod("Is","String","String"));
            TermPartIsUriMethodRef=ModuleDefinition.Import(termPartType.FindMethod("Is","Uri"));
            PropertyMapTypeRef=ModuleDefinition.Import(propertyMapType);
            CompilerGeneratedAttributeCtorRef=ModuleDefinition.Import(compilerGeneratedAttributeType.GetConstructors().Single());

            ObjectFieldRef=ModuleDefinition.Import(rdfVocabularyType.Fields.Single(f => f.Name=="object"));
            PredicateFieldRef=ModuleDefinition.Import(rdfVocabularyType.Fields.Single(f => f.Name=="predicate"));
        }
    }
}