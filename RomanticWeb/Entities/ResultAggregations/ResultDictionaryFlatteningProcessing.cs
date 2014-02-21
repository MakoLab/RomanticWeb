using System;
using System.Collections;
using System.Collections.Generic;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class ResultDictionaryFlatteningProcessing:IResultProcessingStrategy
    {
        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.Dictionary;
            }
        }

        public object Process(IEnumerable<object> objects)
        {
            IDictionary dictionary=new Hashtable();
            FlattenResults(objects,ref dictionary);
            return dictionary;
        }

        private static void FlattenResults(IEnumerable<object> items,ref IDictionary dictionary)
        {
            foreach (var item in items)
            {
                if (item is IEnumerable<object>)
                {
                    FlattenResults((IEnumerable<object>)item,ref dictionary);
                }
                else if (typeof(IDictionary<,>).IsAssignableFromSpecificGeneric(item.GetType()))
                {
                    FlattenResults((IEnumerable)item,ref dictionary);
                }
                else if (item is IDictionary)
                {
                    FlattenResults((IEnumerable)item,ref dictionary);
                }
                else
                {
                    FlattenResults((IEnumerable)new object[] { item },ref dictionary);
                }
            }
        }

        private static void FlattenResults(IEnumerable items,ref IDictionary dictionary)
        {
            foreach (var item in items)
            {
                if (typeof(KeyValuePair<,>).IsAssignableFromSpecificGeneric(item.GetType()))
                {
                    dictionary.Add(item.GetType().GetProperty("Key").GetValue(item),item.GetType().GetProperty("Value").GetValue(item));
                }
                else if (item is DictionaryEntry)
                {
                    dictionary.Add(((DictionaryEntry)item).Key,((DictionaryEntry)item).Value);
                }
                else
                {
                    throw new InvalidCastException(System.String.Format(
                        "Cannot convert item of type '{0}' neither into a '{1}' nor '{2}'",
                        item,
                        typeof(DictionaryEntry),
                        typeof(KeyValuePair<,>)));
                }
            }
        }
    }
}