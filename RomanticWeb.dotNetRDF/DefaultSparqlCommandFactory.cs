using System.Collections.Generic;
using RomanticWeb.Updates;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    public class DefaultSparqlCommandFactory : ISparqlCommandFactory
    {
        public IEnumerable<SparqlUpdateCommand> CreateCommandSet(IEnumerable<DatasetChange> changes)
        {
            yield break;
        }
    }
}