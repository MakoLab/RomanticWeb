using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The Friend of a Friend vocabulary (http://xmlns.com/foaf/0.1/).</summary>
    public static class Foaf
    {
#pragma warning disable 1591
        public const string BaseUri="http://xmlns.com/foaf/0.1/";

        public static readonly Uri Person=new Uri(BaseUri+"Person");

        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        public static readonly Uri primaryTopic=new Uri(BaseUri+"primaryTopic");
#pragma warning restore 1591
    }
}