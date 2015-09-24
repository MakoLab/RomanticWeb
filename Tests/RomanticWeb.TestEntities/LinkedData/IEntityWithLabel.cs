using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LinkedData
{
    public interface IEntityWithLabel : IEntity
    {
        [Property("rdfs", "label")]
        string Label { get; set; }
    }
}