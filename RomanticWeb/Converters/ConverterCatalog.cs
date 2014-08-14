using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Default implementation of <see cref="IConverterCatalog"/>
    /// </summary>
    public sealed class ConverterCatalog : IConverterCatalog
    {
        private static readonly Lazy<IReadOnlyCollection<LiteralNodeConverter>> LiteralConverters;
        private static readonly Lazy<IReadOnlyCollection<INodeConverter>> ComplexConverters;

        static ConverterCatalog()
        {
            LiteralConverters = new Lazy<IReadOnlyCollection<LiteralNodeConverter>>(GetLiteralConverters);
            ComplexConverters = new Lazy<IReadOnlyCollection<INodeConverter>>(GetComplexConverters);
        }

        internal ConverterCatalog()
        {
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<INodeConverter> UriNodeConverters
        {
            get
            {
                return ComplexConverters.Value;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters
        {
            get
            {
                return LiteralConverters.Value;
            }
        }

        private static IReadOnlyCollection<INodeConverter> GetComplexConverters()
        {
            return new ReadOnlyCollection<INodeConverter>(ContainerFactory.GetInstancesImplementing<INodeConverter>().ToList());
        }

        private static IReadOnlyCollection<LiteralNodeConverter> GetLiteralConverters()
        {
            return new ReadOnlyCollection<LiteralNodeConverter>(ContainerFactory.GetInstancesImplementing<LiteralNodeConverter>().ToList());
        }
    }
}