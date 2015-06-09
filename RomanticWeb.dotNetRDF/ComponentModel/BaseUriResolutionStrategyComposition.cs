using RomanticWeb.LinkedData;
using RomanticWeb.Mapping;

namespace RomanticWeb.ComponentModel
{
    /// <summary>Provides dependencies for the <see cref="UrlMatchingResourceResolutionStrategy" /> to run.</summary>
    public class BaseUriResolutionStrategyComposition : CompositionRootBase
    {
        /// <summary>Initializes a new instance of the <see cref="BaseUriResolutionStrategyComposition" /> class.</summary>
        public BaseUriResolutionStrategyComposition()
        {
            MappingModelVisitor<BaseUriMappingModelVisitor>();
        }
    }
}