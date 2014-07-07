using System;
using System.Linq;
using System.Net;

namespace RomanticWeb.IO
{
    /// <summary>Retrieves a content type from the response headers if possible.</summary>
    public class HeadersContentTypeResolver : IContentTypeResolver
    {
        /// <summary>Resolves a MIME type of the given content.</summary>
        /// <param name="uri">Original Uri of the content.</param>
        /// <param name="response">Response object containing content stream.</param>
        /// <returns>String representing a MIME type of the given content.</returns>
        public string Resolve(Uri uri, WebResponse response)
        {
            string result = null;
            if (response != null)
            {
                result = response.Headers.AllKeys.Where(item => item == "Content-Type").Select(item => response.Headers[item]).FirstOrDefault();
            }

            return result;
        }
    }
}