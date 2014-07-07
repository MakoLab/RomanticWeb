using System;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// Implementation of <see cref="IBaseUriSelectionPolicy"/>,
    /// which always returns the same base <see cref="Uri"/> 
    /// </summary>
    public class ConstantBaseUri : IBaseUriSelectionPolicy
    {
        private readonly Uri _defaultBaseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantBaseUri"/> class.
        /// </summary>
        /// <param name="defaultBaseUri">The default base URI.</param>
        /// <exception cref="System.ArgumentException">Base URI must be absolute;defaultBaseUri</exception>
        public ConstantBaseUri(Uri defaultBaseUri)
        {
            if (!defaultBaseUri.IsAbsoluteUri)
            {
                throw new ArgumentException("Base URI must be absolute", "defaultBaseUri");
            }

            _defaultBaseUri = defaultBaseUri;
        }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        public Uri BaseUri
        {
            get
            {
                return _defaultBaseUri;
            }
        }

        /// <summary>
        /// Selects the base URI.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        public Uri SelectBaseUri(EntityId entityId)
        {
            return _defaultBaseUri;
        }
    }
}