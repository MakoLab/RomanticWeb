using System;

namespace RomanticWeb
{
    public sealed class NamespaceSpecification
    {
        public NamespaceSpecification(string prefix, string baseUri)
        {
            Prefix = prefix;
            BaseUri = new Uri(baseUri);
        }

        public Uri BaseUri { get; private set; }

        public string Prefix { get; private set; }
    }
}