using System;

namespace RomanticWeb.Entities
{
    public class ConstantBaseUri:IBaseUriSelectionPolicy
    {
        private readonly Uri _defaultBaseUri;

        public ConstantBaseUri(Uri defaultBaseUri)
        {
            if (!defaultBaseUri.IsAbsoluteUri)
            {
                throw new ArgumentException("Base URI must be absolute","defaultBaseUri");
            }

            _defaultBaseUri=defaultBaseUri;
        }

        public Uri BaseUri
        {
            get
            {
                return _defaultBaseUri;
            }
        }

        public Uri SelectBaseUri(EntityId entityId)
        {
            return _defaultBaseUri;
        }
    }
}