using RomanticWeb.Entities;

namespace RomanticWeb
{
    // todo: maybe pass minimal blank node neighbourhood instead of IEnitytStore
    public interface IBlankNodeConverter
    {
        object Convert(IEntity objectNode, IEntityStore entityStore);

        bool CanConvert(IEntity blankNode, IEntityStore entityStore);
    }
}