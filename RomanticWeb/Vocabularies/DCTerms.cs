using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The DCMI metadata terms (http://purl.org/dc/terms/).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules","*",Justification="Reviewed. Suppression is OK here.")]
    public static class DCTerms
    {
#pragma warning disable 1591
        public const string BaseUri="http://purl.org/dc/terms/";

        public static readonly Uri identifier=new Uri(BaseUri+"identifier");
#pragma warning restore 1591
    }
}