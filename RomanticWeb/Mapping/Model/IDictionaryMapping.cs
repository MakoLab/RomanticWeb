using System;
using System.Diagnostics;
using NullGuard;

namespace RomanticWeb.Mapping.Model
{
    public interface IDictionaryMapping:IPropertyMapping
    {
        Uri KeyPredicate { get; }
        
        Uri ValuePredicate { get; }
    }

    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Dictionary {Name}")]
    internal class DictionaryMapping:PropertyMapping,IDictionaryMapping
    {
        public static readonly Uri DefaultKey=Vocabularies.Rdf.predicate;
        public static readonly Uri DefaultValue=Vocabularies.Rdf.@object;

        public DictionaryMapping(Type returnType,string name,Uri predicateUri)
            :base(returnType,name,predicateUri)
        {
            KeyPredicate=DefaultKey;
            ValuePredicate=DefaultValue;
        }

        public Uri KeyPredicate { get; internal set; }

        public Uri ValuePredicate { get; internal set; }
    }
}