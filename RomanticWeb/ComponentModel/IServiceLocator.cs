using System;

namespace RomanticWeb.ComponentModel
{
    public interface IServiceLocator
    {
        T GetService<T>();

        object GetService(Type serviceType);
    }
}