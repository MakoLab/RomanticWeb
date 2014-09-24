using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using NullGuard;
using RomanticWeb.IO;
using RomanticWeb.LightInject;
using RomanticWeb.Net;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a centralized access to ontology provider factories.</summary>
    public class OntologyFactory
    {
        private readonly IDictionary<string, IOntologyLoader> _ontologyFactoryMimeTypeMappingCache;
        private readonly IServiceContainer _container;
        private readonly ContentTypeResolver _contentTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="OntologyFactory"/> class.
        /// </summary>
        public OntologyFactory()
        {
            _container = new ServiceContainer();
            _container.RegisterAssembly(GetType().Assembly);
            _ontologyFactoryMimeTypeMappingCache = new Dictionary<string, IOntologyLoader>();
            _contentTypeResolver = new ContentTypeResolver(_container.GetAllInstances<IContentTypeResolver>());
        }

        /// <summary>Creates an ontology from given file path.</summary>
        /// <param name="path">File path containing a serialized ontology data.</param>
        /// <remarks>This method assumes that path can be converted to an URI, thus it is possible to pass both local file system and remote files.</remarks>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public Ontology Create(string path)
        {
            return Create(new Uri(path), null);
        }

        /// <summary>Creates an ontology from given file path.</summary>
        /// <param name="path">File path containing a serialized ontology data.</param>
        /// <param name="contentType">Explicitly passed content type of the data stored in the given stream.</param>
        /// <remarks>This method assumes that path can be converted to an URI, thus it is possible to pass both local file system and remote files.</remarks>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public Ontology Create(string path, string contentType)
        {
            return Create(new Uri(path), null);
        }

        /// <summary>Creates an ontology from given stream.</summary>
        /// <param name="fileStream">Stream containing a serialized ontology data.</param>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public Ontology Create(Stream fileStream)
        {
            string contentType = _contentTypeResolver.Resolve(null, new StreamWebResponse(fileStream));
            return Create(fileStream, contentType);
        }

        /// <summary>Creates an ontology from given stream.</summary>
        /// <param name="fileStream">Stream containing a serialized ontology data.</param>
        /// <param name="contentType">Explicitly passed content type of the data stored in the given stream.</param>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        public Ontology Create(Stream fileStream, string contentType)
        {
            IOntologyLoader ontologyFactory = GetOntologyFactory(contentType);
            if (ontologyFactory == null)
            {
                throw new NotSupportedException(System.String.Format("MIME type of '{0}' is not supported.", contentType));
            }

            Ontology result = ontologyFactory.Create(fileStream);
            fileStream.Close();
            return result;
        }

        private Ontology Create(Uri uriPath, [AllowNull] string contentType)
        {
            WebRequest request = WebRequest.Create(uriPath);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            if (contentType == null)
            {
                contentType = _contentTypeResolver.Resolve(uriPath, response);
            }

            return Create(responseStream, contentType);
        }

        private IOntologyLoader GetOntologyFactory(string contentType)
        {
            IOntologyLoader result = null;
            lock (_ontologyFactoryMimeTypeMappingCache)
            {
                if (!_ontologyFactoryMimeTypeMappingCache.TryGetValue(contentType, out result))
                {
                    _ontologyFactoryMimeTypeMappingCache[contentType] = result = _container.GetAllInstances<IOntologyLoader>().Where(item => item.Accepts.Any(mimeType => mimeType == contentType)).FirstOrDefault();
                }
            }

            return result;
        }
    }
}