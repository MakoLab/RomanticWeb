using System.Configuration;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Configuration element for in-memory triple store connected with a file source.
    /// </summary>
    public class FileStoreElement : StoreElement
    {
        private const string FilePathAttributeName = "filePath";

        /// <summary>
        /// Gets or sets the dataset file path.
        /// </summary>
        [ConfigurationProperty(FilePathAttributeName, IsRequired = true)]
        public string FilePath
        {
            get { return (string)this[FilePathAttributeName]; }
            set { this[FilePathAttributeName] = value; }
        }

        /// <inheritdoc />
        public override ITripleStore CreateTripleStore()
        {
            return new FileTripleStore(FilePath);
        }
    }
}