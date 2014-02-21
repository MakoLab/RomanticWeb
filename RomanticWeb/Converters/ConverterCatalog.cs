using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RomanticWeb.ComponentModel.Composition;

namespace RomanticWeb.Converters
{
    public sealed class ConverterCatalog:IConverterCatalog
    {
        private static readonly Lazy<IReadOnlyCollection<ILiteralNodeConverter>> LiteralConverters;
        private static readonly Lazy<IReadOnlyCollection<IComplexTypeConverter>> ComplexConverters;

        static ConverterCatalog()
        {
            LiteralConverters=new Lazy<IReadOnlyCollection<ILiteralNodeConverter>>(GetLiteralConverters);
            ComplexConverters = new Lazy<IReadOnlyCollection<IComplexTypeConverter>>(GetComplexConverters);
        }

        internal ConverterCatalog()
        {
        }

        public IReadOnlyCollection<IComplexTypeConverter> ComplexTypeConverters
        {
            get
            {
                return ComplexConverters.Value;
            }
        }

        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters
        {
            get
            {
                return LiteralConverters.Value;
            }
        }

        private static IReadOnlyCollection<IComplexTypeConverter> GetComplexConverters()
        {
            return new ReadOnlyCollection<IComplexTypeConverter>(ContainerFactory.GetInstancesImplementing<IComplexTypeConverter>().ToList());
        }

        private static IReadOnlyCollection<ILiteralNodeConverter> GetLiteralConverters()
        {
            return new ReadOnlyCollection<ILiteralNodeConverter>(ContainerFactory.GetInstancesImplementing<ILiteralNodeConverter>().ToList());
        }
    }
}