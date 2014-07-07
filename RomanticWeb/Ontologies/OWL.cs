using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The OWL 2 Schema vocabulary (OWL 2) (http://www.w3.org/2002/07/owl#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Owl
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://www.w3.org/2002/07/owl#";

        public static readonly Uri AllDifferent = new Uri(BaseUri + "AllDifferent");

        public static readonly Uri AllDisjointClasses = new Uri(BaseUri + "AllDisjointClasses");

        public static readonly Uri AllDisjointProperties = new Uri(BaseUri + "AllDisjointProperties");

        public static readonly Uri Annotation = new Uri(BaseUri + "Annotation");

        public static readonly Uri AnnotationProperty = new Uri(BaseUri + "AnnotationProperty");

        public static readonly Uri AsymmetricProperty = new Uri(BaseUri + "AsymmetricProperty");

        public static readonly Uri Axiom = new Uri(BaseUri + "Axiom");

        public static readonly Uri Class = new Uri(BaseUri + "Class");

        public static readonly Uri DataRange = new Uri(BaseUri + "DataRange");

        public static readonly Uri DatatypeProperty = new Uri(BaseUri + "DatatypeProperty");

        public static readonly Uri DeprecatedClass = new Uri(BaseUri + "DeprecatedClass");

        public static readonly Uri DeprecatedProperty = new Uri(BaseUri + "DeprecatedProperty");

        public static readonly Uri FunctionalProperty = new Uri(BaseUri + "FunctionalProperty");

        public static readonly Uri InverseFunctionalProperty = new Uri(BaseUri + "InverseFunctionalProperty");

        public static readonly Uri IrreflexiveProperty = new Uri(BaseUri + "IrreflexiveProperty");

        public static readonly Uri NamedIndividual = new Uri(BaseUri + "NamedIndividual");

        public static readonly Uri NegativePropertyAssertion = new Uri(BaseUri + "NegativePropertyAssertion");

        public static readonly Uri ObjectProperty = new Uri(BaseUri + "ObjectProperty");

        public static readonly Uri Ontology = new Uri(BaseUri + "Ontology");

        public static readonly Uri OntologyProperty = new Uri(BaseUri + "OntologyProperty");

        public static readonly Uri ReflexiveProperty = new Uri(BaseUri + "ReflexiveProperty");

        public static readonly Uri Restriction = new Uri(BaseUri + "Restriction");

        public static readonly Uri SymmetricProperty = new Uri(BaseUri + "SymmetricProperty");

        public static readonly Uri TransitiveProperty = new Uri(BaseUri + "TransitiveProperty");

        public static readonly Uri Nothing = new Uri(BaseUri + "Nothing");

        public static readonly Uri Thing = new Uri(BaseUri + "Thing");

        public static readonly Uri allValuesFrom = new Uri(BaseUri + "allValuesFrom");

        public static readonly Uri annotatedProperty = new Uri(BaseUri + "annotatedProperty");

        public static readonly Uri annotatedSource = new Uri(BaseUri + "annotatedSource");

        public static readonly Uri annotatedTarget = new Uri(BaseUri + "annotatedTarget");

        public static readonly Uri assertionProperty = new Uri(BaseUri + "assertionProperty");

        public static readonly Uri cardinality = new Uri(BaseUri + "cardinality");

        public static readonly Uri complementOf = new Uri(BaseUri + "complementOf");

        public static readonly Uri datatypeComplementOf = new Uri(BaseUri + "datatypeComplementOf");

        public static readonly Uri differentFrom = new Uri(BaseUri + "differentFrom");

        public static readonly Uri disjointUnionOf = new Uri(BaseUri + "disjointUnionOf");

        public static readonly Uri disjointWith = new Uri(BaseUri + "disjointWith");

        public static readonly Uri distinctMembers = new Uri(BaseUri + "distinctMembers");

        public static readonly Uri equivalentClass = new Uri(BaseUri + "equivalentClass");

        public static readonly Uri equivalentProperty = new Uri(BaseUri + "equivalentProperty");

        public static readonly Uri hasKey = new Uri(BaseUri + "hasKey");

        public static readonly Uri hasSelf = new Uri(BaseUri + "hasSelf");

        public static readonly Uri hasValue = new Uri(BaseUri + "hasValue");

        public static readonly Uri intersectionOf = new Uri(BaseUri + "intersectionOf");

        public static readonly Uri inverseOf = new Uri(BaseUri + "inverseOf");

        public static readonly Uri maxCardinality = new Uri(BaseUri + "maxCardinality");

        public static readonly Uri maxQualifiedCardinality = new Uri(BaseUri + "maxQualifiedCardinality");

        public static readonly Uri members = new Uri(BaseUri + "members");

        public static readonly Uri minCardinality = new Uri(BaseUri + "minCardinality");

        public static readonly Uri minQualifiedCardinality = new Uri(BaseUri + "minQualifiedCardinality");

        public static readonly Uri onClass = new Uri(BaseUri + "onClass");

        public static readonly Uri onDataRange = new Uri(BaseUri + "onDataRange");

        public static readonly Uri onDatatype = new Uri(BaseUri + "onDatatype");

        public static readonly Uri oneOf = new Uri(BaseUri + "oneOf");

        public static readonly Uri onProperties = new Uri(BaseUri + "onProperties");

        public static readonly Uri onProperty = new Uri(BaseUri + "onProperty");

        public static readonly Uri propertyChainAxiom = new Uri(BaseUri + "propertyChainAxiom");

        public static readonly Uri propertyDisjointWith = new Uri(BaseUri + "propertyDisjointWith");

        public static readonly Uri qualifiedCardinality = new Uri(BaseUri + "qualifiedCardinality");

        public static readonly Uri sameAs = new Uri(BaseUri + "sameAs");

        public static readonly Uri someValuesFrom = new Uri(BaseUri + "someValuesFrom");

        public static readonly Uri sourceIndividual = new Uri(BaseUri + "sourceIndividual");

        public static readonly Uri targetIndividual = new Uri(BaseUri + "targetIndividual");

        public static readonly Uri targetValue = new Uri(BaseUri + "targetValue");

        public static readonly Uri unionOf = new Uri(BaseUri + "unionOf");

        public static readonly Uri withRestrictions = new Uri(BaseUri + "withRestrictions");

        public static readonly Uri bottomDataProperty = new Uri(BaseUri + "bottomDataProperty");

        public static readonly Uri topDataProperty = new Uri(BaseUri + "topDataProperty");

        public static readonly Uri bottomObjectProperty = new Uri(BaseUri + "bottomObjectProperty");

        public static readonly Uri topObjectProperty = new Uri(BaseUri + "topObjectProperty");

        public static readonly Uri backwardCompatibleWith = new Uri(BaseUri + "backwardCompatibleWith");

        public static readonly Uri deprecated = new Uri(BaseUri + "deprecated");

        public static readonly Uri incompatibleWith = new Uri(BaseUri + "incompatibleWith");

        public static readonly Uri priorVersion = new Uri(BaseUri + "priorVersion");

        public static readonly Uri versionInfo = new Uri(BaseUri + "versionInfo");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}