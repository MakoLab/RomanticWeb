using System;

namespace RomanticWeb.Mapping.Model
{
    internal class ClassMapping:IClassMapping
    {
        public ClassMapping(Uri uri,IGraphSelectionStrategy graphSelector)
        {
            GraphSelector=graphSelector;
            Uri=uri;
        }

        public Uri Uri { get; private set; }

        public IGraphSelectionStrategy GraphSelector { get; private set; }
    }
}