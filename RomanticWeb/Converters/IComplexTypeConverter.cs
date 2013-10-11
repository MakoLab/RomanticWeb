using RomanticWeb.Entities;

namespace RomanticWeb.Converters
{
    // todo: maybe pass minimal blank node neighbourhood instead of IEnitytStore
    public interface IComplexTypeConverter
    {
        object Convert(IEntity objectNode, IEntityStore entityStore);

        bool CanConvert(IEntity blankNode, IEntityStore entityStore);
    }
}