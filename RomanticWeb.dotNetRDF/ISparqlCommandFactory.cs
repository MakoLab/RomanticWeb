using System.Collections.Generic;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    public interface ISparqlCommandFactory
    {
        IEnumerable<SparqlUpdateCommand> CreateCommandSet(IEnumerable<Updates.DatasetChange> changes);
    }
}