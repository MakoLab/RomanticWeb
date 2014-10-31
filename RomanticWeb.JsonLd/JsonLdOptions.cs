using System;

namespace RomanticWeb.JsonLd
{
    /// <summary>Describes a JSON-LD processing options.</summary>
    public class JsonLdOptions
    {
        /// <summary>Gets or sets base Uri of the document.</summary>
        public Uri BaseUri { get; set; }

        /// <summary>Gets or sets an expansion context.</summary>
        public string ExpandContext { get; set; }
    }
}