namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// Marker interface. The generic should be used to implement conventions
    /// </summary>
    public interface IConvention
    {
    }

    /// <summary>
    /// A base constract for implementing conventions
    /// </summary>
    /// <typeparam name="T">type this convention should be applied to</typeparam>
    public interface IConvention<in T> : IConvention
    {
        /// <summary>
        /// Checks if convention should be applied
        /// </summary>
        bool ShouldApply(T target);

        /// <summary>
        /// Applies the convention to target
        /// </summary>
        void Apply(T target);
    }
}