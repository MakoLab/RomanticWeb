using System;

namespace RomanticWeb
{
    /// <summary>
    /// Exposes methods to define fallback base URI for relative paths
    /// </summary>
    public class DefaultUriBuilder
    {
        private readonly BaseUriSelectorBuilder _baseUriSelectorBuilder;

        internal DefaultUriBuilder(BaseUriSelectorBuilder baseUriSelectorBuilder)
        {
            _baseUriSelectorBuilder = baseUriSelectorBuilder;
        }

        /// <summary>
        /// Sets the default <see cref="Uri"/>
        /// </summary>
        public void Is(Uri defaultBaseUri)
        {
            _baseUriSelectorBuilder.DefaultBaseUri = defaultBaseUri;
        }
    }
}