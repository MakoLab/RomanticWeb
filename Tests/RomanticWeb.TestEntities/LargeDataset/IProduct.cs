using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LargeDataset
{
    [Class("chem", "Product")]
    public interface IProduct : IEntity
    {
        [Property("schema", "name")]
        string Name { get; set; }

        [Property("rdfs", "comment")]
        string Comments { get; set; }

        [Collection("chem", "viscosityComplex")]
        IViscosityComplex Viscosity { get; }

        [Property("chem", "cureSystem")]
        EntityId CureSystem { get; set; }

        [Property("chem", "cureComplex")]
        ICollection<ICureComplex> CureComplex { get; set; }

        [Collection("chem", "cureTemperature")]
        ICollection<IQuantitativeFloatProperty> CureTemperature { get; }

        [Collection("chem", "cureTime")]
        ICollection<IQuantitativeFloatProperty> CureTime { get; }

        [Collection("chem", "durometer")]
        ICollection<IQuantitativeFloatProperty> Durometer { get; set; }

        [Collection("chem", "tensile")]
        ICollection<IQuantitativeFloatProperty> Tensile { get; set; }

        [Collection("chem", "elongation")]
        ICollection<IQuantitativeFloatProperty> Elongation { get; set; }

        [Property("chem", "tear")]
        IQuantitativeFloatProperty Tear { get; set; }

        [Collection("chem", "rheology")]
        ICollection<IQuantitativeFloatProperty> Rheology { get; set; }

        [Collection("chem", "specificGravity")]
        ICollection<double> SpecificGravity { get; set; }

        [Property("chem", "industrySegment")]
        EntityId Industry { get; set; }

        [Collection("chem", "grade")]
        ICollection<EntityId> Grade { get; set; }

        [Collection("chem", "productCategory")]
        IEnumerable<EntityId> ProductCategory { get; }

        [Collection("chem", "msdsFile")]
        ICollection<IEntity> MsdsFile { get; }

        [Collection("chem", "function")]
        ICollection<EntityId> Function { get; }

        [Collection("chem", "tackFreeTime")]
        ICollection<IQuantitativeFloatProperty> TackFreeTime { get; }

        [Collection("chem", "mixRatio")]
        ICollection<string> MixRatio { get; }

        [Collection("chem", "group")]
        IEnumerable<string> Group { get; set; }

        [Collection("chem", "appearance")]
        ICollection<string> Appearance { get; set; }
    }
}