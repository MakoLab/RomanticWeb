using System.Reflection;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    public sealed class DictionaryMap:PropertyMapBase<DictionaryMap>
    {
        private readonly TermMap _keyMap;
        private readonly TermMap _valueMap;

        internal DictionaryMap(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
            _keyMap=new KeyMap();
            _valueMap=new ValueMap();
        }

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
        public override ITermPart<DictionaryMap> Term
        {
            get
            {
                return new TermPart<DictionaryMap>(this);
            }
        }

        public ITermPart<DictionaryMap> KeyPredicate
        {
            get
            {
                return new DictionaryPart(this,_keyMap);
            }
        }

        public ITermPart<DictionaryMap> ValuePredicate
        {
            get
            {
                return new DictionaryPart(this,_valueMap);
            }
        }

        public override IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            return new DictionaryMapping(PropertyInfo.PropertyType,PropertyInfo.Name,GetTermUri(mappingContext.OntologyProvider));
        }

        public class ValueMap : TermMap { }

        public class KeyMap : TermMap { }

        private class DictionaryPart:ITermPart<DictionaryMap>
        {
            private readonly DictionaryMap _dictionaryMap;
            private readonly TermMap _actualMap;

            internal DictionaryPart(DictionaryMap dictionaryMap,TermMap actualMap)
            {
                _dictionaryMap=dictionaryMap;
                _actualMap=actualMap;
            }

            public DictionaryMap Is(string prefix, string predicateName)
            {
                _actualMap.SetQName(prefix,predicateName);
                return _dictionaryMap;
            }

            public DictionaryMap Is(System.Uri uri)
            {
                _actualMap.SetUri(uri);
                return _dictionaryMap;
            }
        }
    }
}