using System.Collections.Generic;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Class("life", "Carnivore")]
    public interface ICarnivore : IAnimal
    {
        [Collection("life", "animalEaten")]
        IList<IAnimal> Prey { get; }
    }
}