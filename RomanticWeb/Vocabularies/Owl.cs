using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    public static class Owl
    {
        public const string BaseUri="http://www.w3.org/2002/07/owl#";

        public static readonly Uri Thing=new Uri(BaseUri+"Thing");

        public static readonly Uri Class=new Uri(BaseUri+"Class");

        public static readonly Uri Ontology=new Uri(BaseUri+"Ontology");

        public static readonly Uri Restriction=new Uri(BaseUri+"Restriction");

        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        public static readonly Uri disjointWith=new Uri(BaseUri+"disjointWith");

        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        public static readonly Uri onProperty=new Uri(BaseUri+"onProperty");

        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules","SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",Justification="Reviewed. Suppression is OK here.")]
        public static readonly Uri hasValue=new Uri(BaseUri+"hasValue");
    }
}