﻿namespace Nancy.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public class DefaultXmlSerializer : ISerializer
    {
        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="contentType">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanSerialize(string contentType)
        {
            return IsXmlType(contentType);
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        public IEnumerable<string> Extensions
        {
            get { yield return "xml"; }
        }

        /// <summary>
        /// Serialize the given model with the given contentType
        /// </summary>
        /// <param name="contentType">Content type to serialize into</param>
        /// <param name="model">Model to serialize</param>
        /// <param name="outputStream">Output stream to serialize to</param>
        /// <returns>Serialised object</returns>
        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            var serializer = new XmlSerializer(typeof(TModel));
            serializer.Serialize(outputStream, model);
        }

        private static bool IsXmlType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/xml", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.Equals("text/xml", StringComparison.InvariantCultureIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) &&
                   contentMimeType.EndsWith("+xml", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}