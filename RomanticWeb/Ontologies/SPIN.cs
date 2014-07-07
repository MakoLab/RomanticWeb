using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>SPIN Modeling Vocabulary (http://spinrdf.org/spin#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static partial class Spin
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://spinrdf.org/spin#";

        public static readonly Uri Function = new Uri(BaseUri + "Function");

        public static readonly Uri TableDataProvider = new Uri(BaseUri + "TableDataProvider");

        public static readonly Uri ConstructTemplate = new Uri(BaseUri + "ConstructTemplate");

        public static readonly Uri AskTemplate = new Uri(BaseUri + "AskTemplate");

        public static readonly Uri UpdateTemplate = new Uri(BaseUri + "UpdateTemplate");

        public static readonly Uri RuleProperty = new Uri(BaseUri + "RuleProperty");

        public static readonly Uri ConstraintViolation = new Uri(BaseUri + "ConstraintViolation");

        public static readonly Uri Modules = new Uri(BaseUri + "Modules");

        public static readonly Uri SelectTemplate = new Uri(BaseUri + "SelectTemplate");

        public static readonly Uri Column = new Uri(BaseUri + "Column");

        public static readonly Uri LibraryOntology = new Uri(BaseUri + "LibraryOntology");

        public static readonly Uri MagicProperty = new Uri(BaseUri + "MagicProperty");

        public static readonly Uri update = new Uri(BaseUri + "update");

        public static readonly Uri returnType = new Uri(BaseUri + "returnType");

        public static readonly Uri column = new Uri(BaseUri + "column");

        public static readonly Uri symbol = new Uri(BaseUri + "symbol");

        public static readonly Uri violationRoot = new Uri(BaseUri + "violationRoot");

        public static readonly Uri columnType = new Uri(BaseUri + "columnType");

        public static readonly Uri nextRuleProperty = new Uri(BaseUri + "nextRuleProperty");

        public static readonly Uri @private = new Uri(BaseUri + "private");

        public static readonly Uri labelTemplate = new Uri(BaseUri + "labelTemplate");

        public static readonly Uri violationPath = new Uri(BaseUri + "violationPath");

        public static readonly Uri constructor = new Uri(BaseUri + "constructor");

        public static readonly Uri @abstract = new Uri(BaseUri + "abstract");

        public static readonly Uri constraint = new Uri(BaseUri + "constraint");

        public static readonly Uri fix = new Uri(BaseUri + "fix");

        public static readonly Uri columnWidth = new Uri(BaseUri + "columnWidth");

        public static readonly Uri violationSource = new Uri(BaseUri + "violationSource");

        public static readonly Uri columnIndex = new Uri(BaseUri + "columnIndex");

        public static readonly Uri thisUnbound = new Uri(BaseUri + "thisUnbound");

        public static readonly Uri rulePropertyMaxIterationCount = new Uri(BaseUri + "rulePropertyMaxIterationCount");

        public static readonly Uri eval = new Uri(BaseUri + "eval");

        public static readonly Uri ConstructTemplates = new Uri(BaseUri + "ConstructTemplates");

        public static readonly Uri AskTemplates = new Uri(BaseUri + "AskTemplates");

        public static readonly Uri UpdateTemplates = new Uri(BaseUri + "UpdateTemplates");

        public static readonly Uri SelectTemplates = new Uri(BaseUri + "SelectTemplates");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}