using System;
using System.Reflection;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for dictionary property predicate
    /// </summary>
    public class DictionaryMappingProvider:PropertyMappingProvider,IDictionaryMappingProvider
    {
        private readonly ITermMappingProvider _key;
        private readonly ITermMappingProvider _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMappingProvider"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="key">The key mapping provider.</param>
        /// <param name="value">The value mapping provider.</param>
        /// <param name="property">The property.</param>
        public DictionaryMappingProvider(Uri uri,ITermMappingProvider key,ITermMappingProvider value,PropertyInfo property)
            :base(uri,property)
        {
            _key=key;
            _value=value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMappingProvider"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="term">The term.</param>
        /// <param name="key">The key mapping provider.</param>
        /// <param name="value">The value mapping provider.</param>
        /// <param name="property">The property.</param>
        public DictionaryMappingProvider(string prefix,string term,ITermMappingProvider key,ITermMappingProvider value,PropertyInfo property)
            : base(prefix,term,property)
        {
            _key=key;
            _value=value;
        }

        /// <summary>
        /// Gets the key mapping provider.
        /// </summary>
        /// <value>
        /// The key mapping provider.
        /// </value>
        public ITermMappingProvider Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Gets the value mapping provider.
        /// </summary>
        /// <value>
        /// The value mapping provider.
        /// </value>
        public ITermMappingProvider Value
        {
            get
            {
                return _value;
            }
        }

        /// <inheridoc/>
        public override void Accept(Visitors.IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }
    }
}