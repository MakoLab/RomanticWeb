using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    internal interface ITripleStoreFactory
    {
        ITripleStore CreateTripleStore();
    }
}