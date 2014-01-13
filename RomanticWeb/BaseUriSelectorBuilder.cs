using System;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    /// <summary>
    /// Exposes methods to define how base <see cref="Uri"/>s
    /// are selected for relative identifiers
    /// </summary>
    public class BaseUriSelectorBuilder
    {
        private Uri _defaultBaseUri;

        /// <summary>
        /// Gets the builder for default base <see cref="Uri"/>.
        /// </summary>
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