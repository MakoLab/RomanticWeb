using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>Provides a basic default graph selection overriding mechanism.</summary>
    public class OverridingGraphSelector : ISourceGraphSelectionOverride
    {
        private readonly Func<INamedGraphSelector, Uri> _selectGraph;

        /// <summary>Creates an instance of the <see cref="OverridingGraphSelector"/>.</summary>
        /// <param name="entityId">Target entity identifier.</param>
        /// <param name="entityMapping">Target entity mapping.</param>
        /// <param name="propertyMapping">Target property mapping</param>
        public OverridingGraphSelector(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping propertyMapping)
        {
            _selectGraph = selector => selector.SelectGraph(entityId, entityMapping, propertyMapping);
        }

        /// <summary>Gets the selected graph.</summary>
        public virtual Func<INamedGraphSelector, Uri> SelectGraph
        {
            get
            {
                return _selectGraph;
            }
        }
    }
}