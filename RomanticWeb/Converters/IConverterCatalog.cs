using System.Collections.Generic;

namespace RomanticWeb.Converters
{
    public interface IConverterCatalog
    {
        IReadOnlyCollection<IComplexTypeConverter> ComplexTypeConverters { get; }

        IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters { get; }
    }
}