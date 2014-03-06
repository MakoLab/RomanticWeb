namespace RomanticWeb.Mapping.Conventions
{
    public interface IConvention
    {
    }

    public interface IConvention<in T>:IConvention
    {
        bool ShouldApply(T target);

        void Apply(T target);
    }
}