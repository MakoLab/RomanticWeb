using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LargeDataset
{
    [Class("schema", "QuantitativeValue")]
    public interface IQuantitativeFloatProperty : IEntity
    {
        [Property("schema", "unitCode")]
        string Unit { get; set; }

        [Property("schema", "value")]
        double? Value { get; set; }
    }
}
