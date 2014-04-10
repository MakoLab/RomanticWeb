using System;
using System.Configuration;
using NullGuard;

namespace RomanticWeb.Configuration
{
    public class BaseUriElement:ConfigurationElement
    {
        public const string DefaultUriAttributeName="default";

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