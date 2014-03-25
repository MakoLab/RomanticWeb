using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace RomanticWeb.Fody
{
    internal class WeaverImports
    {
        private readonly ModuleWeaver _moduleWeaver;
        private readonly WeaverReferences _references;

        public WeaverImports(ModuleWeaver moduleWeaver,WeaverReferences references)
        {
            _moduleWeaver=moduleWeaver;
            _references=references;
            Cache=new DictionaryCache();
        }

        public MethodReference PropertyMapIsUriRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(PropertyMapTypeRef));
            }
        }

        public MethodReference PropertyMapIsQNameRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartIsQNameMethodRef.Resolve().MakeHostInstanceGeneric(PropertyMapTypeRef));
            }
        }

        public MethodReference CollectionMapIsUriRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(CollectionMapTypeRef));
            }
        }

        public MethodReference CollectionMapIsQNameRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartIsQNameMethodRef.Resolve().MakeHostInstanceGeneric(CollectionMapTypeRef));
            }
        }

        public TypeReference DictionaryAttributeTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("DictionaryAttribute"));
            }
        }

        public TypeReference KeyAttributeTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("KeyAttribute"));
            }
        }

        public TypeReference ValueAttributeTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("ValueAttribute"));
            }
        }

        public TypeReference SystemTypeRef
        {
            get
            {
                return ModuleDefinition.Import(typeof(Type));
            }
        }

        public TypeReference DictionaryEntryTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("IDictionaryEntry`2"));
            }
        }

        public TypeReference DictionaryOwnerTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("IDictionaryOwner`3"));
            }
        }

        public TypeReference CollectionMapTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("CollectionMap"));
            }
        }

        public TypeReference TermPartTypeRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartType);
            }
        }

        public TypeReference DictionaryOwnerMapTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("DictionaryOwnerMap`4"));
            }
        }

        public TypeReference DictionaryEntryMapTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("DictionaryEntryMap`3"));
            }
        }

        public TypeReference PropertyMapTermPartTypeRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartType.MakeGenericInstanceType(PropertyMapType));
            }
        }

        public TypeReference PropertyMapTypeRef
        {
            get
            {
                return ModuleDefinition.Import(PropertyMapType);
            }
        }

        public MethodReference CompilerGeneratedAttributeCtorRef
        {
            get
            {
                return ModuleDefinition.Import(CompilerGeneratedAttributeType.GetConstructors().Single());
            }
        }

        public MethodReference TermPartIsQNameMethodRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartType.FindMethod("Is","String","String"));
            }
        }

        public MethodReference TermPartIsUriMethodRef
        {
            get
            {
                return ModuleDefinition.Import(TermPartType.FindMethod("Is","Uri"));
            }
        }

        public FieldReference ObjectFieldRef
        {
            get
            {
                return ModuleDefinition.Import(RdfVocabularyType.Fields.Single(f => f.Name=="object"));
            }
        }

        public FieldReference PredicateFieldRef
        {
            get
            {
                return ModuleDefinition.Import(RdfVocabularyType.Fields.Single(f => f.Name=="predicate"));
            }
        }

        public MethodReference UriStringConstructorRef
        {
            get
            {
                return ModuleDefinition.Import(ModuleDefinition.Import(typeof(Uri)).Resolve().FindConstructor("String"));
            }
        }

        public TypeDefinition TermPartType
        {
            get
            {
                return _references.Orm.FindType("ITermPart`1");
            }
        }

        public TypeReference EntityMapTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("EntityMap"));
            }
        }

        public TypeReference EntityTypeRef
        {
            get
            {
                return ModuleDefinition.Import(_references.Orm.FindType("IEntity"));
            }
        }

        public TypeReference DictionaryTypeRef
        {
            get
            {
                return ModuleDefinition.Import(typeof(IDictionary<,>));
            }
        }

        private ModuleDefinition ModuleDefinition
        {
            get
            {
                return _moduleWeaver.ModuleDefinition;
            }
        }

        private TypeDefinition PropertyMapType
        {
            get
            {
                return _references.Orm.FindType("PropertyMap");
            }
        }

        private TypeDefinition RdfVocabularyType
        {
            get
            {
                return _references.Orm.FindType("RomanticWeb.Vocabularies.Rdf");
            }
        }

        private TypeDefinition CompilerGeneratedAttributeType
        {
            get
            {
                return ModuleDefinition.Import(typeof(CompilerGeneratedAttribute)).Resolve();
            }
        }

        private DictionaryCache Cache { get; set; }
    }
}