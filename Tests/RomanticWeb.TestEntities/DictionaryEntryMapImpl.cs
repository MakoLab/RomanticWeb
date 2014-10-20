using System;
using RomanticWeb.Collections;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities
{
    public interface IValue
    {
    }

    public interface IKey
    {
    }

    public interface IDict : IDictionaryEntry<IKey, IValue>
    {
    }

    public class DictionaryEntryMapImpl : DictionaryEntryMap<IDict, IKey, IValue>
    {
        protected override void SetupKeyProperty(ITermPart<IPropertyMap> term)
        {
            term.Is(new Uri("123")).ConvertWith<IntegerConverter>();
        }

        protected override void SetupValueProperty(ITermPart<IPropertyMap> term)
        {
            term.Is(new Uri("123"));
        }
    }
}