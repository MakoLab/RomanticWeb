namespace RomanticWeb.ComponentModel
{
    public interface IServiceLocator
    {
        T GetService<T>();
    }
}