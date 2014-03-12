using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>Dublin Core Metadata Element Set, Version 1.1 (http://purl.org/dc/elements/1.1/).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules","*",Justification="Reviewed. Suppression is OK here.")]
    public static class Dc
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri="http://purl.org/dc/elements/1.1/";

        public static readonly Uri contributor=new Uri(BaseUri+"contributor");

        public static readonly Uri coverage=new Uri(BaseUri+"coverage");

        public static readonly Uri creator=new Uri(BaseUri+"creator");

        public static readonly Uri date=new Uri(BaseUri+"date");

        public static readonly Uri description=new Uri(BaseUri+"description");

        public static readonly Uri format=new Uri(BaseUri+"format");

        public static readonly Uri identifier=new Uri(BaseUri+"identifier");

        public static readonly Uri language=new Uri(BaseUri+"language");

        public static readonly Uri publisher=new Uri(BaseUri+"publisher");

        public static readonly Uri relation=new Uri(BaseUri+"relation");

        public static readonly Uri rights=new Uri(BaseUri+"rights");

        public static readonly Uri source=new Uri(BaseUri+"source");

        public static readonly Uri subject=new Uri(BaseUri+"subject");

        public static readonly Uri title=new Uri(BaseUri+"title");

        public static readonly Uri type=new Uri(BaseUri+"type");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}