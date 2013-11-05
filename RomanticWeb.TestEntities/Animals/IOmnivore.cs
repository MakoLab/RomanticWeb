using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Class("life", "Omnivore")]
    public interface IOmnivore:ICarnivore,IHerbivore
    {
        
    }
}