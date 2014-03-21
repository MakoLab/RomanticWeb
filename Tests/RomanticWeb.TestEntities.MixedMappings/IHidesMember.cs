using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public interface IHidesMember:IGenericParent<int>
    {
        [Property("urn:hidden:mapping")]
        new int MappedProperty1 { get; }

        new int MappedProperty2 { get; }
    }
}