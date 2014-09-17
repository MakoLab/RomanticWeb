namespace RomanticWeb.ComponentModel
{
    public interface IComponentRegistryFacade
    {
        void Register<TService, TComponent>() where TComponent : TService;

        void Register<TService>(TService instance);
    }
}