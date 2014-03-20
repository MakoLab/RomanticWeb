using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    public sealed class DictionaryMap:PropertyMapBase<DictionaryMap>
    {
        private readonly KeyMap _keyMap;

        private readonly ValueMap _valueMap;

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

        public override Aggregation? Aggregation
        {
            get
            {
                return Entities.ResultAggregations.Aggregation.Original;
            }
        }

        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this,_keyMap.Accept(fluentMapsVisitor),_valueMap.Accept(fluentMapsVisitor));
        }

        public class ValueMap:TermMap
        {
            public ITermMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
            {
                return fluentMapsVisitor.Visit(this);
            }
        }

        public class KeyMap:TermMap
        {
            public ITermMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
            {
                return fluentMapsVisitor.Visit(this);
            }
        }

        private class DictionaryPart:ITermPart<DictionaryMap>
        {
            private readonly DictionaryMap _dictionaryMap;
            private readonly TermMap _actualMap;

            internal DictionaryPart(DictionaryMap dictionaryMap,TermMap actualMap)
            {
                _dictionaryMap=dictionaryMap;
                _actualMap=actualMap;
            }

            public DictionaryMap Is(string prefix,string predicateName)
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