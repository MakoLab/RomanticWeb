using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public interface ITripleStoreFactory
    {
        ITripleStore CreateTripleStore();
    }
}