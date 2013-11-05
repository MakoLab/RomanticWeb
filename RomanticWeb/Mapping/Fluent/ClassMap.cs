using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    public sealed class ClassMap
    {
        internal string NamespacePrefix { get; set; }

        internal string ClassName { get; set; }

        public void Is(string prefix,string className)
        {
            NamespacePrefix=prefix;
            ClassName=className;
        }

        internal IClassMapping GetMapping(IOntologyProvider prefixes)
        {
            return new ClassMapping(prefixes.ResolveUri(NamespacePrefix,ClassName));
        }
    }
}