using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for collection properties
    /// </summary>
	public sealed class CollectionMap:PropertyMapBase<CollectionMap>
    {
        private Aggregation? _aggregation;

        internal CollectionMap(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
		}

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
        public override ITermPart<CollectionMap> Term
        {
            get
            {
                return new TermPart<CollectionMap>(this);
            }
        }

        /// <summary>
        /// Gets options for setting how this collection will be persisted
        /// </summary>
        public StorageStrategyPart StoreAs
        {
            get
            {
                return new StorageStrategyPart(this);
            }
        }

        public override Aggregation? Aggregation
        {
            get
            {
                return _aggregation;
            }
        }

        public StoreAs StorageStrategy { get; set; }

        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }
    }
}