using System;
using RomanticWeb.Converters;
using RomanticWeb.LightInject;

namespace RomanticWeb.ComponentModel
{
    internal class ConvertersCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            RegisterConverter<Base64BinaryConverter>(registry);
            RegisterConverter<BooleanConverter>(registry);
            RegisterConverter<DateTimeConverter>(registry);
            RegisterConverter<DecimalConverter>(registry);
            RegisterConverter<DefaultUriConverter>(registry);
            RegisterConverter<DoubleConverter>(registry);
            RegisterConverter<DurationConverter>(registry);
            RegisterConverter<EntityIdConverter>(registry);
            RegisterConverter<GuidConverter>(registry);
            RegisterConverter<IntegerConverter>(registry);
            RegisterConverter<StringConverter>(registry);
            registry.Register(typeof(AsEntityConverter<>), new PerContainerLifetime());
            registry.Register(typeof(EntityIdConverter<>), new PerContainerLifetime());

            registry.Register<FallbackNodeConverter>(new PerContainerLifetime());

            registry.RegisterFallback(
                (type, s) => typeof(INodeConverter).IsAssignableFrom(type),
                request => Activator.CreateInstance(request.ServiceType));
        }

        private void RegisterConverter<T>(IServiceRegistry registry) where T : INodeConverter
        {
            registry.Register<INodeConverter, T>(typeof(T).FullName, new PerContainerLifetime());
            registry.Register<T>(new PerContainerLifetime());
        }
    }
}