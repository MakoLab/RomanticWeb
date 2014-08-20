using System;
using RomanticWeb.ComponentModel;

namespace RomanticWeb.LightInject
{
    internal class ServiceLocator : IServiceLocator
    {
        private readonly IServiceContainer _container;

        public ServiceLocator(IServiceContainer container)
        {
            _container = container;
        }

        public T GetService<T>()
        {
            return _container.GetInstance<T>();
        }

        public object GetService(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }

        public T GetService<T>(string serviceName)
        {
            return _container.GetInstance<T>(serviceName);
        }
    }
}