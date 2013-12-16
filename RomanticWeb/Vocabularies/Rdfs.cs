using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    public static class Rdfs
    {
        public const string BaseUri="http://www.w3.org/2000/01/rdf-schema#";

        public static readonly Uri Class=new Uri(BaseUri+"Class");

        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        public static readonly Uri subClassOf=new Uri(BaseUri+"subClassOf");
    }
}