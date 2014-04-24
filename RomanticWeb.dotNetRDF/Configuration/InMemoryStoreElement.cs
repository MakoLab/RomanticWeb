using System.Configuration;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public class InMemoryStoreElement:StoreElement
    {
        private const string ThreadSafeAttributeName="threadSafe";

        [ConfigurationProperty(ThreadSafeAttributeName,DefaultValue=true)]
        public bool ThreadSafe
        {
            get { return (bool)this[ThreadSafeAttributeName]; }
            set { this[ThreadSafeAttributeName] = value; }
        }

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