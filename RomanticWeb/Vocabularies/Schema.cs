using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The schema.org vocabulary (http://schema.org/).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Schema
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri="http://schema.org/";

        public static readonly Uri text=new Uri(BaseUri+"text");

        public static readonly Uri fileFormat=new Uri(BaseUri+"fileFormat");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}