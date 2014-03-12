using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The Resource Description Framework vocabulary (http://www.w3.org/1999/02/22-rdf-syntax-ns#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static partial class Rdf
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public static readonly Uri about=new Uri(BaseUri+"about");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}