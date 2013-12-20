using System;
using System.Collections.Generic;

namespace RomanticWeb
{
    internal sealed class AbsoluteUriComparer:IComparer<Uri>,IEqualityComparer<Uri>
    {
        public static readonly AbsoluteUriComparer Default=new AbsoluteUriComparer();

        public int Compare(Uri x,Uri y)
        {
            return Uri.Compare(x,y,UriComponents.AbsoluteUri,UriFormat.UriEscaped,StringComparison.Ordinal);
        }

        public bool Equals(Uri x,Uri y)
        {
            return Compare(x,y)==0;
        }

        public int GetHashCode(Uri obj)
        {
            if (obj==null)
            {
                return 0;
            }

            return obj.AbsoluteUri.GetHashCode();
        }
    }
}