using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LargeDataset
{
    [Class("chem","Product")]
    public interface IProduct:IEntity
    {
        [Property("schema","name")]
        string Name { get; set; }

        [Property("rdfs","comment")]
        string Comments { get; set; }

        [Collection("chem","viscosity")]
        ICollection<IQuantitativeFloatProperty> Viscosity { get; }

        [Property("chem","cureSystem")]
        IEntity CureSystem { get; set; }

        [Collection("chem","cureTemperature")]
        ICollection<IQuantitativeFloatProperty> CureTemperature { get; }

        [Collection("chem","cureTime")]
        ICollection<IQuantitativeFloatProperty> CureTime { get; }

        [Property("chem","durometer")]
        IQuantitativeFloatProperty Durometer { get; set; }

        [Property("chem","tensile")]
        IQuantitativeFloatProperty Tensile { get; set; }

        [Property("chem","elongation")]
        IQuantitativeFloatProperty Elongation { get; set; }

        [Property("chem","tear")]
        IQuantitativeFloatProperty Tear { get; set; }

        [Property("chem","rheology")]
        IQuantitativeFloatProperty Rheology { get; set; }

        [Property("chem","specificGravity")]
        double? SpecificGravity { get; set; }

        [Property("chem","industrySegment")]
        EntityId Industry { get; set; }

        [Property("chem","grade")]
        EntityId Grade { get; set; }

        [Property("chem","productCategory")]
        IEnumerable<EntityId> ProductCategory { get; }

        [Collection("chem","msdsFile")]
        ICollection<IEntity> MsdsFile { get; }

        [Collection("chem","function")]
        ICollection<EntityId> Function { get; }

        [Collection("chem","group")]
        IEnumerable<string> Group { get; set; }
    }
}