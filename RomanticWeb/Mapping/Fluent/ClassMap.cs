using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    public sealed class ClassMap:INamedGraphSelectingMap
    {
        /// <summary>
        /// Gets a named graph mapping part
        /// </summary>
        public NamedGraphPart<ClassMap> NamedGraph
        {
            get
            {
                return new NamedGraphPart<ClassMap>(this); 
            }
        }

        IGraphSelectionStrategy INamedGraphSelectingMap.GraphSelector { get; set; }

        internal string NamespacePrefix { get; set; }

        internal string ClassName { get; set; }

        public ClassMap Is(string prefix, string className)
        {
            NamespacePrefix=prefix;
            ClassName=className;
            return this;
        }

        internal IClassMapping GetMapping(IOntologyProvider prefixes)
        {
            return new ClassMapping(
                prefixes.ResolveUri(NamespacePrefix,ClassName),
                ((INamedGraphSelectingMap)this).GraphSelector);
        }
 }
}