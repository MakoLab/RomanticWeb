using System.Collections.Generic;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Class("life","Herbivore")]
    public interface IHerbivore:IAnimal
    {
        [Collection("life","plantEaten")]
        IList<IPlant> Diet { get; }
    }
}