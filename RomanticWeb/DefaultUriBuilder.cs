using System;

namespace RomanticWeb
{
    public class DefaultUriBuilder
    {
        private readonly BaseUriSelectorBuilder _baseUriSelectorBuilder;

        internal DefaultUriBuilder(BaseUriSelectorBuilder baseUriSelectorBuilder)
        {
            _baseUriSelectorBuilder=baseUriSelectorBuilder;
        }

        public void Is(Uri defaultBaseUri)
        {
            _baseUriSelectorBuilder.DefaultBaseUri=defaultBaseUri;
        }
    }
}