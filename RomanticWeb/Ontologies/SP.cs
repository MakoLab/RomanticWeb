using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>SPIN SPARQL Syntax (http://spinrdf.org/sp#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static partial class Sp
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://spinrdf.org/sp#";

        public static readonly Uri Path = new Uri(BaseUri + "Path");

        public static readonly Uri Asc = new Uri(BaseUri + "Asc");

        public static readonly Uri Sum = new Uri(BaseUri + "Sum");

        public static readonly Uri Union = new Uri(BaseUri + "Union");

        public static readonly Uri TriplePattern = new Uri(BaseUri + "TriplePattern");

        public static readonly Uri GroupConcat = new Uri(BaseUri + "GroupConcat");

        public static readonly Uri Load = new Uri(BaseUri + "Load");

        public static readonly Uri DeleteData = new Uri(BaseUri + "DeleteData");

        public static readonly Uri Desc = new Uri(BaseUri + "Desc");

        public static readonly Uri TripleTemplate = new Uri(BaseUri + "TripleTemplate");

        public static readonly Uri Max = new Uri(BaseUri + "Max");

        public static readonly Uri Avg = new Uri(BaseUri + "Avg");

        public static readonly Uri Modify = new Uri(BaseUri + "Modify");

        public static readonly Uri TriplePath = new Uri(BaseUri + "TriplePath");

        public static readonly Uri ElementList = new Uri(BaseUri + "ElementList");

        public static readonly Uri SubQuery = new Uri(BaseUri + "SubQuery");

        public static readonly Uri Min = new Uri(BaseUri + "Min");

        public static readonly Uri Bind = new Uri(BaseUri + "Bind");

        public static readonly Uri Optional = new Uri(BaseUri + "Optional");

        public static readonly Uri AltPath = new Uri(BaseUri + "AltPath");

        public static readonly Uri Count = new Uri(BaseUri + "Count");

        public static readonly Uri ReversePath = new Uri(BaseUri + "ReversePath");

        public static readonly Uri Construct = new Uri(BaseUri + "Construct");

        public static readonly Uri Variable = new Uri(BaseUri + "Variable");

        public static readonly Uri Ask = new Uri(BaseUri + "Ask");

        public static readonly Uri ModPath = new Uri(BaseUri + "ModPath");

        public static readonly Uri Create = new Uri(BaseUri + "Create");

        public static readonly Uri NamedGraph = new Uri(BaseUri + "NamedGraph");

        public static readonly Uri ReverseLinkPath = new Uri(BaseUri + "ReverseLinkPath");

        public static readonly Uri Values = new Uri(BaseUri + "Values");

        public static readonly Uri NotExists = new Uri(BaseUri + "NotExists");

        public static readonly Uri Drop = new Uri(BaseUri + "Drop");

        public static readonly Uri DeleteWhere = new Uri(BaseUri + "DeleteWhere");

        public static readonly Uri InsertData = new Uri(BaseUri + "InsertData");

        public static readonly Uri Service = new Uri(BaseUri + "Service");

        public static readonly Uri Select = new Uri(BaseUri + "Select");

        public static readonly Uri Exists = new Uri(BaseUri + "Exists");

        public static readonly Uri Filter = new Uri(BaseUri + "Filter");

        public static readonly Uri Minus = new Uri(BaseUri + "Minus");

        public static readonly Uri Clear = new Uri(BaseUri + "Clear");

        public static readonly Uri Describe = new Uri(BaseUri + "Describe");

        public static readonly Uri SeqPath = new Uri(BaseUri + "SeqPath");

        public static readonly Uri Sample = new Uri(BaseUri + "Sample");

        public static readonly Uri arg5 = new Uri(BaseUri + "arg5");

        public static readonly Uri path1 = new Uri(BaseUri + "path1");

        public static readonly Uri arg1 = new Uri(BaseUri + "arg1");

        public static readonly Uri @default = new Uri(BaseUri + "default");

        public static readonly Uri values = new Uri(BaseUri + "values");

        public static readonly Uri @object = new Uri(BaseUri + "object");

        public static readonly Uri graphNameNode = new Uri(BaseUri + "graphNameNode");

        public static readonly Uri varName = new Uri(BaseUri + "varName");

        public static readonly Uri named = new Uri(BaseUri + "named");

        public static readonly Uri @as = new Uri(BaseUri + "as");

        public static readonly Uri distinct = new Uri(BaseUri + "distinct");

        public static readonly Uri path2 = new Uri(BaseUri + "path2");

        public static readonly Uri orderBy = new Uri(BaseUri + "orderBy");

        public static readonly Uri variable = new Uri(BaseUri + "variable");

        public static readonly Uri arg4 = new Uri(BaseUri + "arg4");

        public static readonly Uri silent = new Uri(BaseUri + "silent");

        public static readonly Uri having = new Uri(BaseUri + "having");

        public static readonly Uri query = new Uri(BaseUri + "query");

        public static readonly Uri groupBy = new Uri(BaseUri + "groupBy");

        public static readonly Uri graphIRI = new Uri(BaseUri + "graphIRI");

        public static readonly Uri limit = new Uri(BaseUri + "limit");

        public static readonly Uri @using = new Uri(BaseUri + "using");

        public static readonly Uri templates = new Uri(BaseUri + "templates");

        public static readonly Uri resultNodes = new Uri(BaseUri + "resultNodes");

        public static readonly Uri usingNamed = new Uri(BaseUri + "usingNamed");

        public static readonly Uri arg3 = new Uri(BaseUri + "arg3");

        public static readonly Uri reduced = new Uri(BaseUri + "reduced");

        public static readonly Uri subPath = new Uri(BaseUri + "subPath");

        public static readonly Uri into = new Uri(BaseUri + "into");

        public static readonly Uri with = new Uri(BaseUri + "with");

        public static readonly Uri serviceURI = new Uri(BaseUri + "serviceURI");

        public static readonly Uri where = new Uri(BaseUri + "where");

        public static readonly Uri document = new Uri(BaseUri + "document");

        public static readonly Uri resultVariables = new Uri(BaseUri + "resultVariables");

        public static readonly Uri separator = new Uri(BaseUri + "separator");

        public static readonly Uri text = new Uri(BaseUri + "text");

        public static readonly Uri path = new Uri(BaseUri + "path");

        public static readonly Uri modMax = new Uri(BaseUri + "modMax");

        public static readonly Uri bindings = new Uri(BaseUri + "bindings");

        public static readonly Uri elements = new Uri(BaseUri + "elements");

        public static readonly Uri predicate = new Uri(BaseUri + "predicate");

        public static readonly Uri node = new Uri(BaseUri + "node");

        public static readonly Uri fromNamed = new Uri(BaseUri + "fromNamed");

        public static readonly Uri arg2 = new Uri(BaseUri + "arg2");

        public static readonly Uri subject = new Uri(BaseUri + "subject");

        public static readonly Uri deletePattern = new Uri(BaseUri + "deletePattern");

        public static readonly Uri expression = new Uri(BaseUri + "expression");

        public static readonly Uri all = new Uri(BaseUri + "all");

        public static readonly Uri from = new Uri(BaseUri + "from");

        public static readonly Uri offset = new Uri(BaseUri + "offset");

        public static readonly Uri varNames = new Uri(BaseUri + "varNames");

        public static readonly Uri modMin = new Uri(BaseUri + "modMin");

        public static readonly Uri insertPattern = new Uri(BaseUri + "insertPattern");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}