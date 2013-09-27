using System;

namespace RomanticWeb.Mapping.Model
{
    internal class ClassMapping:IClassMapping
    {
        public ClassMapping(Uri uri)
        {
            Uri=uri;
        }

        public Uri Uri { get; private set; }
    }
}