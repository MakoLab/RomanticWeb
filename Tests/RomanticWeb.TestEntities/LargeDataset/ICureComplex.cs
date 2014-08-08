using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LargeDataset
{
    [Class("chem", "Cure")]
    public interface ICureComplex : IEntity
    {
        [Collection("chem", "cureTemperature")]
        ICollection<IQuantitativeFloatProperty> CureTemperature { get; set; }

        [Collection("chem", "cureTime")]
        ICollection<IQuantitativeFloatProperty> CureTime { get; set; }
    }
}