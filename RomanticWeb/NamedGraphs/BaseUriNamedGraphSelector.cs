using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>Provides a named graph according to given base uris.</summary>
    /// <remarks>This selector should be used for read-only graph per multiple resources scenario.</remarks>
    public class BaseUriNamedGraphSelector : GraphSelectionStrategyBase
    {
        private readonly IEnumerable<Uri> _baseUris;

        /// <summary>Initializes a new instance of the <see cref="BaseUriNamedGraphSelector" /> class.</summary>
        /// <param name="baseUris">Base uris.</param>
        public BaseUriNamedGraphSelector(params Uri[] baseUris) : this((IEnumerable<Uri>)baseUris)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BaseUriNamedGraphSelector" /> class.</summary>
        /// <param name="baseUris">Base uris.</param>
        public BaseUriNamedGraphSelector(IEnumerable<Uri> baseUris)
        {
            if (baseUris == null)
            {
                throw new ArgumentNullException("baseUris");
            }

            if (!baseUris.Any())
            {
                throw new ArgumentOutOfRangeException("baseUris");
            }

            _baseUris = baseUris;
        }

        /// <inheritdoc />
        protected override Uri GetGraphForEntityId(EntityId entityId, [AllowNull] IEntityMapping entityMapping, [AllowNull] IPropertyMapping predicate)
        {
            return (_baseUris.FirstOrDefault(uri => entityId.Uri.AbsoluteUri.StartsWith(uri.AbsoluteUri)) ?? entityId.Uri);
        }
    }
}