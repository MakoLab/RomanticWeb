using System;
using System.Collections.Generic;

namespace RomanticWeb
{
    internal class AbsoluteUriComparer:IComparer<Uri>
    {
        public int Compare(Uri x,Uri y)
        {
            return Uri.Compare(x,y,UriComponents.AbsoluteUri,UriFormat.UriEscaped,StringComparison.Ordinal);
        }
    }
}