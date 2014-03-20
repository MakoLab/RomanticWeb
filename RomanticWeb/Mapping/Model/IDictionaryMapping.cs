using System;

namespace RomanticWeb.Mapping.Model
{
    public interface IDictionaryMapping:IPropertyMapping
    {
        Uri KeyPredicate { get; }
        
        Uri ValuePredicate { get; }
    }
}