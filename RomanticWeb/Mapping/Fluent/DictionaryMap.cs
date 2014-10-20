using System.Reflection;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for dictionary properties
    /// </summary>
    public sealed class DictionaryMap : PropertyMapBase, IDictionaryMap
    {
        private readonly KeyMap _keyMap;

        private readonly ValueMap _valueMap;

        internal DictionaryMap(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            _keyMap = new KeyMap();
            _valueMap = new ValueMap();
        }

        /// <inheritdoc/>
        public ITermPart<IDictionaryMap> Term
        {
            get
            {
                return new TermPart<DictionaryMap>(this);
            }
        }

        /// <summary>
        /// Gets the dictionary key predicate map part
        /// </summary>
        public ITermPart<IDictionaryMap> KeyPredicate
        {
            get
            {
                return new DictionaryPart(this, _keyMap);
            }
        }

        /// <summary>
        /// Gets the dictionary value predicate map part
        /// </summary>
        public ITermPart<IDictionaryMap> ValuePredicate
        {
            get
            {
                return new DictionaryPart(this, _valueMap);
            }
        }

        public IDictionaryMap ConvertKeysWith<TConverter>() where TConverter : INodeConverter
        {
            return this;
        }

        public IDictionaryMap ConvertValuesWith<TConverter>() where TConverter : INodeConverter
        {
            return this;
        }

        /// <inheritdoc/>
        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this, _keyMap.Accept(fluentMapsVisitor), _valueMap.Accept(fluentMapsVisitor));
        }

        /// <summary>
        /// A definition for mapping dictionary value predicate
        /// </summary>
        public class ValueMap : TermMap
        {
            /// <summary>
            /// Accepts the specified fluent maps visitor.
            /// </summary>
            public IPredicateMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
            {
                return fluentMapsVisitor.Visit(this);
            }
        }

        /// <summary>
        /// A definition for mapping dictionary key predicate
        /// </summary>
        public class KeyMap : TermMap
        {
            /// <summary>
            /// Accepts the specified fluent maps visitor.
            /// </summary>
            public IPredicateMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
            {
                return fluentMapsVisitor.Visit(this);
            }
        }

        private class DictionaryPart : ITermPart<DictionaryMap>
        {
            private readonly DictionaryMap _dictionaryMap;
            private readonly TermMap _actualMap;

            internal DictionaryPart(DictionaryMap dictionaryMap, TermMap actualMap)
            {
                _dictionaryMap = dictionaryMap;
                _actualMap = actualMap;
            }

            public DictionaryMap Is(string prefix, string predicateName)
            {
                _actualMap.SetQName(prefix, predicateName);
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