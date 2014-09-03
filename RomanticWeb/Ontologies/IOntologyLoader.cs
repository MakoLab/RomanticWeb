using System.IO;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a base interface for ontology factories.</summary>
    public interface IOntologyLoader
    {
        /// <summary>Gets an array of accepted MIME types.</summary>
        string[] Accepts { get; }

        /// <summary>Creates an ontology from given stream.</summary>
        /// <param name="fileStream">Stream containing a serialized ontology data.</param>
        /// <returns>Ontology beeing an object representation of given data.</returns>
        Ontology Create(Stream fileStream);
    }
}