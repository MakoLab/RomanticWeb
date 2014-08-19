using System;
using RomanticWeb.Converters;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping.Conventions;

namespace RomanticWeb
{
    internal class ConventionsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IConvention, CollectionStorageConvention>("CollectionStorageConvention", new PerContainerLifetime());
            serviceRegistry.Register<IConvention, DefaultDictionaryKeyPredicateConvention>("DefaultDictionaryKeyPredicateConvention", new PerContainerLifetime());
            serviceRegistry.Register<IConvention, DefaultDictionaryValuePredicateConvention>("DefaultDictionaryValuePredicateConvention", new PerContainerLifetime());
            serviceRegistry.Register<IConvention, RdfListConvention>("RdfListConvention", new PerContainerLifetime());
            serviceRegistry.Register<IConvention, EntityPropertiesConvention>("EntityPropertiesConvention", new PerContainerLifetime());
            serviceRegistry.Register<IConvention, EntityIdPropertiesConvention>("EntityIdPropertiesConvention", new PerContainerLifetime());

            // todo: introduce a kind of builder so that changing configuration is easy
            var converters = new DefaultConvertersConvention()
                .SetDefault<IntegerConverter>(IntegerConverter.SupportedTypes)
                .SetDefault<DecimalConverter>(typeof(decimal))
                .SetDefault<BooleanConverter>(typeof(bool))
                .SetDefault<Base64BinaryConverter>(typeof(byte[]))
                .SetDefault<DateTimeConverter>(typeof(DateTime))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<GuidConverter>(typeof(Guid))
                .SetDefault<DefaultUriConverter>(typeof(Uri))
                .SetDefault<StringConverter>(typeof(string))
                .SetDefault<FallbackNodeConverter>(typeof(object));
            serviceRegistry.RegisterInstance<IConvention>(converters, "DefaultConvertersConvention");
        }
    }
}