using System;
using System.Net;

namespace RomanticWeb.IO
{
    /// <summary>Provides an unified base infrastructure for resolving content type.</summary>
    public interface IContentTypeResolver
    {
        /// <summary>Resolves a MIME type of the given content.</summary>
        /// <param name="uri">Original Uri of the content.</param>
        /// <param name="response">Response object containing content stream.</param>
        /// <returns>String representing a MIME type of the given content.</returns>
        string Resolve(Uri uri, WebResponse response);
    }
}