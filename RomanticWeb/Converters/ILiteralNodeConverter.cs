using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    public interface ILiteralNodeConverter:INodeConverter
    {
        LiteralConversionMatch CanConvert(Node literalNode);
    }
}