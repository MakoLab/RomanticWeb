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

        [Property("chem","viscosity")]
        IQuantitativeFloatProperty Viscosity { get; set; }

        [Property("chem","cureSystem")]
        IEntity CureSystem { get; set; }

        [Property("chem","cureTemperature")]
        IQuantitativeFloatProperty CureTemperature { get; set; }

        [Property("chem","cureTime")]
        IQuantitativeFloatProperty CureTime { get; set; }

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
        IEntity ProductCategory { get; set; }

        [Collection("chem","msdsFile")]
        IEnumerable<IEntity> MsdsFile { get; set; }

        [Collection("chem","function")]
        IEnumerable<EntityId> Function { get; set; }
    }
}