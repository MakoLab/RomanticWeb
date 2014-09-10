using System.Collections.Generic;
using RomanticWeb.Updates;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    public interface ISparqlCommandFactory
    {
        IEnumerable<SparqlUpdateCommand> CreateCommands(DatasetChange change);
    }
}