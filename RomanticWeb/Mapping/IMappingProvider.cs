using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    public interface IMappingProvider
	{
        IMapping GetMapping(IOntologyProvider prefixes);
	}
}