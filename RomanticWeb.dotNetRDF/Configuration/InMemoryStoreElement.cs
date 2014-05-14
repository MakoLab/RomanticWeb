using System.Configuration;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Configuration element for in-memory triple store
    /// </summary>
    public class InMemoryStoreElement:StoreElement
    {
        private const string ThreadSafeAttributeName="threadSafe";

        /// <summary>
        /// Gets or sets a value indicating whether the store should be thread safe.
        /// </summary>
        [ConfigurationProperty(ThreadSafeAttributeName,DefaultValue=true)]
        public bool ThreadSafe
        {
            get { return (bool)this[ThreadSafeAttributeName]; }
            set { this[ThreadSafeAttributeName] = value; }
        }

        /// <inheritdoc />
        public override ITripleStore CreateTripleStore()
        {
            if (ThreadSafe)
            {
                return new ThreadSafeTripleStore();
            }

            return new TripleStore();
        }
    }
}