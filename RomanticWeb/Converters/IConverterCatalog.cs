using System.Collections.Generic;

namespace RomanticWeb.Converters
{
    public interface IConverterCatalog
    {
        IReadOnlyCollection<IUriNodeConverter> ComplexTypeConverters { get; }

        IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters { get; }
    }
}