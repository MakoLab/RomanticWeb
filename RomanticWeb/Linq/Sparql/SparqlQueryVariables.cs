using NullGuard;

namespace RomanticWeb.Linq.Sparql
{
    /// <summary>
    /// Represents variables used in Romantic Web's wueries
    /// </summary>
    [NullGuard(ValidationFlags.None)]
    public class SparqlQueryVariables
    {
        private readonly string _entity;
        private readonly string _subject;
        private readonly string _predicate;
        private readonly string _object;
        private readonly string _metaGraph;
        private readonly string _owner;
        private readonly string _scalar;

        internal SparqlQueryVariables(string entity, string subject, string predicate, string @object, string owner, string metaGraph, string scalar)
        {
            _entity = entity;
            _subject = subject;
            _predicate = predicate;
            _object = @object;
            _owner = owner;
            _metaGraph = metaGraph;
            _scalar = scalar;
        }

        /// <summary>
        /// Gets the entity variable name.
        /// </summary>
        public string Entity
        {
            get
            {
                return _entity;
            }
        }

        /// <summary>
        /// Gets the subject variable name.
        /// </summary>
        public string Subject
        {
            get
            {
                return _subject;
            }
        }

        /// <summary>
        /// Gets the predicate variable name.
        /// </summary>
        public string Predicate
        {
            get
            {
                return _predicate;
            }
        }

        /// <summary>
        /// Gets the object variable name.
        /// </summary>
        public string Object
        {
            get
            {
                return _object;
            }
        }

        /// <summary>
        /// Gets the owning entity variable name.
        /// </summary>
        public string Owner
        {
            get
            {
                return _owner;
            }
        }

        /// <summary>
        /// Gets the metagraph variable name.
        /// </summary>
        public string MetaGraph
        {
            get
            {
                return _metaGraph;
            }
        }

        /// <summary>
        /// Gets the scalar variable name.
        /// </summary>
        public string Scalar
        {
            get
            {
                return _scalar;
            }
        }
    }
}