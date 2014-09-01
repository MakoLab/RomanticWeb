using System;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Conventions;

namespace RomanticWeb.ComponentModel
{
    internal sealed class ConventionsCompositionRoot : CompositionRootBase
    {
        public ConventionsCompositionRoot()
        {
            Convention<CollectionStorageConvention>();
            Convention<DefaultDictionaryKeyPredicateConvention>();
            Convention<DefaultDictionaryValuePredicateConvention>();
            Convention<RdfListConvention>();
            Convention<EntityPropertiesConvention>();
            Convention<EntityIdPropertiesConvention>();
            Convention<DefaultConvertersConvention>(SetDefaultConverters);
        }

        private static void SetDefaultConverters(DefaultConvertersConvention convention)
        {
            convention
                .SetDefault<IntegerConverter>(IntegerConverter.SupportedTypes)
                .SetDefault<DecimalConverter>(typeof(decimal))
                .SetDefault<BooleanConverter>(typeof(bool))
                .SetDefault<Base64BinaryConverter>(typeof(byte[]))
                .SetDefault<DateTimeConverter>(typeof(DateTime))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<GuidConverter>(typeof(Guid))
                .SetDefault<DefaultUriConverter>(typeof(Uri))
                .SetDefault<StringConverter>(typeof(string))
                .SetDefault<FallbackNodeConverter>(typeof(object));
        }
    }
}