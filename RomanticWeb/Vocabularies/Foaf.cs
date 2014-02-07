using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The Friend of a Friend vocabulary (http://xmlns.com/foaf/0.1/).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Foaf
    {
        // ReSharper disable InconsistentNaming
#pragma warning disable 1591
        public const string BaseUri="http://xmlns.com/foaf/0.1/";

        public static readonly Uri Person=new Uri(BaseUri+"Person");

        public static readonly Uri primaryTopic=new Uri(BaseUri+"primaryTopic");
#pragma warning restore 1591
        // ReSharper restore InconsistentNaming
    }
}