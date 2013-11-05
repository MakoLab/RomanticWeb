using System.Collections.Generic;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Class("life", "Herbivore")]
    public interface IHerbivore:IAnimal
    {
        [Property("life", "plantEaten", IsCollection = true)]
        IList<IPlant> Diet { get; }
    }
}