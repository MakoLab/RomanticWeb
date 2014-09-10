using System.Collections.Generic;
using RomanticWeb.Updates;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>
    /// Defines the contract for creating SPARQL Update commands
    /// </summary>
    public interface ISparqlCommandFactory
    {
        /// <summary>
        /// Creates the commands represented by the <paramref name="change"/>
        /// </summary>
        IEnumerable<SparqlUpdateCommand> CreateCommands(DatasetChange change);
    }
}