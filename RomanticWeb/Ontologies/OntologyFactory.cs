using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.IO;
using RomanticWeb.Net;
using RomanticWeb.Ontologies;

namespace Magi.Data
{
    public class OntologyFactory
    {
        private static IEnumerable<IOntologyFactory> OntologyFactories;

        static OntologyFactory()
        {
            OntologyFactories=ContainerFactory.GetInstancesImplementing<IOntologyFactory>();
        }

        public static Ontology Create(string path)
        {
            if (path==null)
            {
                throw new ArgumentNullException("path");
            }

            Uri uriPath=new Uri(path);
            WebRequest request=WebRequest.Create(uriPath);
            WebResponse response=request.GetResponse();
            Stream responseStream=response.GetResponseStream();
            string contentType=ContentTypeResolver.Resolve(uriPath,response);
            return Create(responseStream,contentType);
        }

        public static Ontology Create(Stream fileStream)
        {
            if (fileStream==null)
            {
                throw new ArgumentNullException("fileStream");
            }

            string contentType=ContentTypeResolver.Resolve(null,new StreamWebResponse(fileStream));
            return Create(fileStream,contentType);
        }

        public static Ontology Create(Stream fileStream,string contentType)
        {
            if (fileStream==null)
            {
                throw new ArgumentNullException("fileStream");
            }

            if (contentType==null)
            {
                throw new ArgumentNullException("contentType");
            }

            IOntologyFactory ontologyFactory=OntologyFactories.Where(item => item.Accepts.Any(mimeType => mimeType==contentType)).FirstOrDefault();
            if (ontologyFactory==null)
            {
                throw new NotSupportedException(System.String.Format("MIME type of '{0}' is not supported.",contentType));
            }

            Ontology result=ontologyFactory.Create(fileStream);
            fileStream.Close();
            return result;
        }
    }
}