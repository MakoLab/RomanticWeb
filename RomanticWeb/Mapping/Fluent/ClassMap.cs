using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for rdf class
    /// </summary>
    public sealed class ClassMap : TermMap,INamedGraphSelectingMap
    {
        private readonly TermPart<ClassMap> _termPart;

        internal ClassMap()
        {
            _termPart=new TermPart<ClassMap>(this);
        }

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

        /// <inheritdoc />
        IGraphSelectionStrategy INamedGraphSelectingMap.GraphSelector { get; set; }

        /// <summary>
        /// Sets the class name 
        /// </summary>
        public ClassMap Is(string prefix, string className)
        {
            return _termPart.Is(prefix, className);
        }

        /// <summary>
        /// Sets the class name 
        /// </summary>
        public ClassMap Is(Uri uri)
        {
            return _termPart.Is(uri);
        }

        internal IClassMapping GetMapping(MappingContext mappingContext)
        {
            return new ClassMapping(
                GetTermUri(mappingContext.OntologyProvider),
                ((INamedGraphSelectingMap)this).GraphSelector ?? mappingContext.DefaultGraphSelector);
        }
 }
}