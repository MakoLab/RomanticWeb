using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LargeDataset
{
    [Class("chem", "Viscosity")]
    public interface IViscosityComplex : IEntity
    {
        [Collection("chem", "viscosity")]
        ICollection<IQuantitativeFloatProperty> Viscosity { get; set; }

        [Collection("chem", "viscosityPartA")]
        ICollection<IQuantitativeFloatProperty> PartA { get; set; }

        [Collection("chem", "viscosityPartB")]
        ICollection<IQuantitativeFloatProperty> PartB { get; set; }
    }
}