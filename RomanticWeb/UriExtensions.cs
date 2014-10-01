using System;

namespace RomanticWeb
{
    /// <summary>Provides useful Uri extensions.</summary>
    public static class UriExtensions
    {
        /// <summary>Returns fragment or last segment as entity name.</summary>
        /// <param name="uri">Uri to be parsed.</param>
        /// <returns><see cref="EntityName" /> instance or <b>null</b> if the passed uri is also null.</returns>
        public static string GetFragmentOrLastSegment(this Uri uri)
        {
            string result = null;
            if (uri != null)
            {
                string segment;
                string uriString = uri.ToString();
                int position = uriString.IndexOf('#');
                if ((position > 0) && ((segment = uriString.Substring(position + 1)).Length > 0))
                {
                    result = segment;
                }
                else
                {
                    position = uriString.IndexOf('?');
                    segment = (position > 0 ? uriString.Substring(0, position) : uriString);
                    if (uri.IsAbsoluteUri)
                    {
                        segment = segment.Substring(uri.Scheme.Length + 1);
                        if (segment.StartsWith("//"))
                        {
                            segment = segment.Substring(2);
                        }
                    }

                    string[] segments = segment.Split('/');
                    result = segments[segments.Length - 1];
                }
            }

            return result;
        }
    }
}
