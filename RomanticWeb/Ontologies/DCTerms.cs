using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>DCMI metadata terms (http://purl.org/dc/terms/).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules","*",Justification="Reviewed. Suppression is OK here.")]
    public static class DCTerms
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri="http://purl.org/dc/terms/";

        public static readonly Uri Agent=new Uri(BaseUri+"Agent");

        public static readonly Uri AgentClass=new Uri(BaseUri+"AgentClass");

        public static readonly Uri BibliographicResource=new Uri(BaseUri+"BibliographicResource");

        public static readonly Uri FileFormat=new Uri(BaseUri+"FileFormat");

        public static readonly Uri Frequency=new Uri(BaseUri+"Frequency");

        public static readonly Uri Jurisdiction=new Uri(BaseUri+"Jurisdiction");

        public static readonly Uri LicenseDocument=new Uri(BaseUri+"LicenseDocument");

        public static readonly Uri LinguisticSystem=new Uri(BaseUri+"LinguisticSystem");

        public static readonly Uri Location=new Uri(BaseUri+"Location");

        public static readonly Uri LocationPeriodOrJurisdiction=new Uri(BaseUri+"LocationPeriodOrJurisdiction");

        public static readonly Uri MediaType=new Uri(BaseUri+"MediaType");

        public static readonly Uri MediaTypeOrExtent=new Uri(BaseUri+"MediaTypeOrExtent");

        public static readonly Uri MethodOfAccrual=new Uri(BaseUri+"MethodOfAccrual");

        public static readonly Uri MethodOfInstruction=new Uri(BaseUri+"MethodOfInstruction");

        public static readonly Uri PeriodOfTime=new Uri(BaseUri+"PeriodOfTime");

        public static readonly Uri PhysicalMedium=new Uri(BaseUri+"PhysicalMedium");

        public static readonly Uri PhysicalResource=new Uri(BaseUri+"PhysicalResource");

        public static readonly Uri Policy=new Uri(BaseUri+"Policy");

        public static readonly Uri ProvenanceStatement=new Uri(BaseUri+"ProvenanceStatement");

        public static readonly Uri RightsStatement=new Uri(BaseUri+"RightsStatement");

        public static readonly Uri SizeOrDuration=new Uri(BaseUri+"SizeOrDuration");

        public static readonly Uri Standard=new Uri(BaseUri+"Standard");

        public static readonly Uri @abstract=new Uri(BaseUri+"abstract");

        public static readonly Uri accessRights=new Uri(BaseUri+"accessRights");

        public static readonly Uri accrualMethod=new Uri(BaseUri+"accrualMethod");

        public static readonly Uri accrualPeriodicity=new Uri(BaseUri+"accrualPeriodicity");

        public static readonly Uri accrualPolicy=new Uri(BaseUri+"accrualPolicy");

        public static readonly Uri alternative=new Uri(BaseUri+"alternative");

        public static readonly Uri audience=new Uri(BaseUri+"audience");

        public static readonly Uri available=new Uri(BaseUri+"available");

        public static readonly Uri bibliographicCitation=new Uri(BaseUri+"bibliographicCitation");

        public static readonly Uri conformsTo=new Uri(BaseUri+"conformsTo");

        public static readonly Uri contributor=new Uri(BaseUri+"contributor");

        public static readonly Uri coverage=new Uri(BaseUri+"coverage");

        public static readonly Uri created=new Uri(BaseUri+"created");

        public static readonly Uri creator=new Uri(BaseUri+"creator");

        public static readonly Uri date=new Uri(BaseUri+"date");

        public static readonly Uri dateAccepted=new Uri(BaseUri+"dateAccepted");

        public static readonly Uri dateCopyrighted=new Uri(BaseUri+"dateCopyrighted");

        public static readonly Uri dateSubmitted=new Uri(BaseUri+"dateSubmitted");

        public static readonly Uri description=new Uri(BaseUri+"description");

        public static readonly Uri educationLevel=new Uri(BaseUri+"educationLevel");

        public static readonly Uri extent=new Uri(BaseUri+"extent");

        public static readonly Uri format=new Uri(BaseUri+"format");

        public static readonly Uri hasFormat=new Uri(BaseUri+"hasFormat");

        public static readonly Uri hasPart=new Uri(BaseUri+"hasPart");

        public static readonly Uri hasVersion=new Uri(BaseUri+"hasVersion");

        public static readonly Uri identifier=new Uri(BaseUri+"identifier");

        public static readonly Uri instructionalMethod=new Uri(BaseUri+"instructionalMethod");

        public static readonly Uri isFormatOf=new Uri(BaseUri+"isFormatOf");

        public static readonly Uri isPartOf=new Uri(BaseUri+"isPartOf");

        public static readonly Uri isReferencedBy=new Uri(BaseUri+"isReferencedBy");

        public static readonly Uri isReplacedBy=new Uri(BaseUri+"isReplacedBy");

        public static readonly Uri isRequiredBy=new Uri(BaseUri+"isRequiredBy");

        public static readonly Uri issued=new Uri(BaseUri+"issued");

        public static readonly Uri isVersionOf=new Uri(BaseUri+"isVersionOf");

        public static readonly Uri language=new Uri(BaseUri+"language");

        public static readonly Uri license=new Uri(BaseUri+"license");

        public static readonly Uri mediator=new Uri(BaseUri+"mediator");

        public static readonly Uri medium=new Uri(BaseUri+"medium");

        public static readonly Uri modified=new Uri(BaseUri+"modified");

        public static readonly Uri provenance=new Uri(BaseUri+"provenance");

        public static readonly Uri publisher=new Uri(BaseUri+"publisher");

        public static readonly Uri references=new Uri(BaseUri+"references");

        public static readonly Uri relation=new Uri(BaseUri+"relation");

        public static readonly Uri replaces=new Uri(BaseUri+"replaces");

        public static readonly Uri requires=new Uri(BaseUri+"requires");

        public static readonly Uri rights=new Uri(BaseUri+"rights");

        public static readonly Uri rightsHolder=new Uri(BaseUri+"rightsHolder");

        public static readonly Uri source=new Uri(BaseUri+"source");

        public static readonly Uri spatial=new Uri(BaseUri+"spatial");

        public static readonly Uri subject=new Uri(BaseUri+"subject");

        public static readonly Uri tableOfContents=new Uri(BaseUri+"tableOfContents");

        public static readonly Uri temporal=new Uri(BaseUri+"temporal");

        public static readonly Uri title=new Uri(BaseUri+"title");

        public static readonly Uri type=new Uri(BaseUri+"type");

        public static readonly Uri valid=new Uri(BaseUri+"valid");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}