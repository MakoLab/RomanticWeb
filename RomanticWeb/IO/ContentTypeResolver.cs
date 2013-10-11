using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RomanticWeb.ComponentModel.Composition;

namespace RomanticWeb.IO
{
    /// <summary>Provides a main source of the content type resolution infrastructure.</summary>
    public class ContentTypeResolver
    {
        private static IEnumerable<IContentTypeResolver> ContentTypeResolvers;

        static ContentTypeResolver()
        {
            ContentTypeResolvers=ContainerFactory.GetInstancesImplementing<IContentTypeResolver>();
        }

        /// <summary>Resolves a MIME type of the given content.</summary>
        /// <param name="uri">Original Uri of the content.</param>
        /// <param name="response">Response object containing content stream.</param>
        /// <returns>String representing a MIME type of the given content.</returns>
        public static string Resolve(Uri uri,WebResponse response)
        {
            return ContentTypeResolvers.Select(item => item.Resolve(uri,response)).FirstOrDefault(item => item!=null);
        }
    }
}