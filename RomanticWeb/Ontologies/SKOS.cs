using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>SKOS Vocabulary (http://www.w3.org/2004/02/skos/core#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Skos
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://www.w3.org/2004/02/skos/core#";
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}