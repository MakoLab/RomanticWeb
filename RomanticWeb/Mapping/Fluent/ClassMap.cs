using System;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for rdf class
    /// </summary>
    public sealed class ClassMap:TermMap
    {
        private readonly TermPart<ClassMap> _termPart;

        internal ClassMap()
        {
            _termPart=new TermPart<ClassMap>(this);
        }

        /// <summary>
        /// Sets the class name 
        /// </summary>
        public ClassMap Is(string prefix,string className)
        {
            return _termPart.Is(prefix,className);
        }

        /// <summary>
        /// Sets the class name 
        /// </summary>
        public ClassMap Is(Uri uri)
        {
            return _termPart.Is(uri);
        }

        public IClassMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }
    }
}