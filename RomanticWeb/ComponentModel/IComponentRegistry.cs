namespace RomanticWeb.ComponentModel
{
    /// <summary>
    /// A façade, which exposes the service container held by the <see cref="EntityContextFactory" />
    /// </summary>
    /// <remarks>It shouldn't be used from consumer code</remarks>
    public interface IComponentRegistryFacade
    {
        /// <summary>
        /// Registers a type with the service container
        /// </summary>
        void Register<TService, TComponent>() where TComponent : TService;

        /// <summary>
        /// Registers an instance with the service container
        /// </summary>
        void Register<TService>(TService instance);
    }
}