using System;

namespace RomanticWeb.Mapping.Model
{
    internal class TypeMapping:ITypeMapping
    {
        public TypeMapping(Uri uri)
        {
            this.Uri=uri;
        }

        public Uri Uri { get; private set; }
    }
}