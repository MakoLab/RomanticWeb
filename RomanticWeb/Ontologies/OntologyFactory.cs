using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.IO;
using RomanticWeb.Net;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a centralized access to ontology provider factories.</summary>
    public class OntologyFactory
    {
        private static IEnumerable<IOntologyFactory> OntologyFactories;
        private static IDictionary<string,IOntologyFactory> OntologyFactoryMimeTypeMappingCache;

        static OntologyFactory()
        {
            OntologyFactories=ContainerFactory.GetInstancesImplementing<IOntologyFactory>();
            OntologyFactoryMimeTypeMappingCache=new Dictionary<string,IOntologyFactory>();
        }

        /// <summary>Creates an ontology from given file path.</summary>
        /// <param name="path">File path containing a serialized ontology data.</param>
        /// <remarks>This method assumes that path can be converted to an URI, thus it is possible to pass both local file system and remote files.</remarks>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public static Ontology Create(string path)
        {
            Uri uriPath=new Uri(path);
            WebRequest request=WebRequest.Create(uriPath);
            WebResponse response=request.GetResponse();
            Stream responseStream=response.GetResponseStream();
            string contentType=ContentTypeResolver.Resolve(uriPath,response);
            return Create(responseStream,contentType);
        }

        /// <summary>Creates an ontology from given stream.</summary>
        /// <param name="fileStream">Stream containing a serialized ontology data.</param>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public static Ontology Create(Stream fileStream)
        {
            if (fileStream==null)
            {
                throw new ArgumentNullException("fileStream");
            }

            string contentType=ContentTypeResolver.Resolve(null,new StreamWebResponse(fileStream));
            return Create(fileStream,contentType);
        }

        /// <summary>Creates an ontology from given stream.</summary>
        /// <param name="fileStream">Stream containing a serialized ontology data.</param>
        /// <param name="contentType">Explicitely passed content type of the data stored in the given stream.</param>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public static Ontology Create(Stream fileStream,string contentType)
        {
            IOntologyFactory ontologyFactory=GetOntologyFactory(contentType);
            if (ontologyFactory==null)
            {
                throw new NotSupportedException(System.String.Format("MIME type of '{0}' is not supported.",contentType));
            }

            Ontology result=ontologyFactory.Create(fileStream);
            fileStream.Close();
            return result;
        }

        private static IOntologyFactory GetOntologyFactory(string contentType)
        {
            IOntologyFactory result=null;
            lock (OntologyFactoryMimeTypeMappingCache)
            {
                if (!OntologyFactoryMimeTypeMappingCache.ContainsKey(contentType))
                {
                    OntologyFactoryMimeTypeMappingCache[contentType]=result=OntologyFactories.Where(item => item.Accepts.Any(mimeType => mimeType==contentType)).FirstOrDefault();
                }
                else
                {
                    result=OntologyFactoryMimeTypeMappingCache[contentType];
                }
            }

            return result;
        }
    }
}