using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.LightInject;

namespace RomanticWeb.ComponentModel
{
    internal class ConvertersCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            registry.Register<IConverterCatalog, ConverterCatalog>(new PerContainerLifetime());
            registry.Register<IFallbackNodeConverter, FallbackNodeConverter>(new PerContainerLifetime());

            registry.Register<IResultTransformerCatalog, ResultTransformerCatalog>(new PerContainerLifetime());
            RegisterResultAggregator<AnyResultCheck>(registry);
            RegisterResultAggregator<FirstOrDefault>(registry);
            RegisterResultAggregator<FirstResult>(registry);
            RegisterResultAggregator<SingleOrDefault>(registry);
            RegisterResultAggregator<SingleResult>(registry);
        }

        private void RegisterResultAggregator<T>(IServiceRegistry registry) where T : IResultAggregator
        {
            registry.Register<IResultAggregator, T>(typeof(T).FullName, new PerContainerLifetime());
        }
    }
}