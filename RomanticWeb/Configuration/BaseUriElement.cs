using System;
using System.Configuration;
using NullGuard;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// Configuration element to set base Uri for
    /// </summary>
    public class BaseUriElement:ConfigurationElement
    {
        private const string DefaultUriAttributeName="default";

        /// <summary>
        /// Gets or sets the default base Uri.
        /// </summary>
        [ConfigurationProperty(DefaultUriAttributeName)]
        [UriValidator]
        public Uri Default
        {
            [return:AllowNull] 
            get { return (Uri)this[DefaultUriAttributeName]; }
            set { this[DefaultUriAttributeName] = value; }
        }
    }
}