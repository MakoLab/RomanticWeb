using System;
using System.Collections.Generic;
using NullGuard;

namespace RomanticWeb
{
    /// <summary>Compares absolute Uris.</summary>
    [NullGuard(ValidationFlags.None)]
    public sealed class AbsoluteUriComparer : IComparer<Uri>, IEqualityComparer<Uri>
    {
        /// <summary>Provides an easy access to the default instance of the <see cref="AbsoluteUriComparer" />.</summary>
        public static readonly AbsoluteUriComparer Default = new AbsoluteUriComparer();

        /// <inheritdoc />
        public int Compare(Uri x, Uri y)
        {
            return Uri.Compare(x, y, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public bool Equals(Uri x, Uri y)
        {
            return Compare(x, y) == 0;
        }

        /// <inheritdoc />
        public int GetHashCode(Uri obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.AbsoluteUri.GetHashCode();
        }
    }
}