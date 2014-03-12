using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    public class OverridingGraphSelector : ISourceGraphSelectionOverride
    {
        private readonly Func<INamedGraphSelector,Uri> _selectGraph;

        public OverridingGraphSelector(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping propertyMapping)
        {
            _selectGraph=selector => selector.SelectGraph(entityId,entityMapping,propertyMapping);
        }

        public virtual Func<INamedGraphSelector,Uri> SelectGraph
        {
            get
            {
                return _selectGraph;
            }
        }
    }
}