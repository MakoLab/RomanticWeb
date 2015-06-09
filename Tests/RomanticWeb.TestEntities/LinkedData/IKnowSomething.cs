using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.LinkedData
{
    public interface IKnowSomething : IEntity
    {
        [Property("urn:i-want-tell")]
        IEntityWithLabel IWontTell { get; set; }
    }
}