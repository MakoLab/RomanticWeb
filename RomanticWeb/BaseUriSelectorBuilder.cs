using System;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    public class BaseUriSelectorBuilder
    {
        private Uri _defaultBaseUri;

        public DefaultUriBuilder Default
        {
            get
            {
                return new DefaultUriBuilder(this);
            }
        }

        internal Uri DefaultBaseUri
        {
            private get
            {
                return _defaultBaseUri;
            }

            set
            {
                if (!value.IsAbsoluteUri)
                {
                    throw new ArgumentException("Base URI must be absolute", "value");
                }

                _defaultBaseUri=value;
            }
        }

        internal IBaseUriSelectionPolicy Build()
        {
            return new ConstantBaseUri(DefaultBaseUri);
        }
    }
}