using System.Collections.Generic;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Class("life", "Carnivore")]
    public interface ICarnivore:IAnimal
    {
        [Property("life", "animalEaten", IsCollection=true)]
        IList<IAnimal> Prey { get; }
    }
}