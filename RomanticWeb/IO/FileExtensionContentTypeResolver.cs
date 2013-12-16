using System;
using System.Net;

namespace RomanticWeb.IO
{
    /// <summary>Assigns a MIME type for a known file type extension.</summary>
    public class FileExtensionContentTypeResolver:IContentTypeResolver
    {
        /// <summary>Resolves a MIME type of the given content.</summary>
        /// <param name="uri">Original Uri of the content.</param>
        /// <param name="response">Response object containing content stream.</param>
        /// <returns>String representing a MIME type of the given content.</returns>
        public string Resolve(Uri uri,WebResponse response)
        {
            string result=null;
            if (uri!=null)
            {
                string extension=System.IO.Path.GetExtension(uri.AbsolutePath);
                if (!System.String.IsNullOrEmpty(extension))
                {
                    switch (extension.ToLower())
                    {
                        case ".owl":
                            result="application/owl+xml";
                            break;
                        case ".ttl":
                            result="text/turtle";
                            break;
                        case ".rdf":
                            result="application/rdf+xml";
                            break;
                    }
                }
            }

            return result;
        }
    }
}