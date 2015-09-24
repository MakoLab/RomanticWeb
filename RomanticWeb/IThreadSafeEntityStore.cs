namespace RomanticWeb
{
    /// <summary>Represents an in-memory quad cache, which are organized in per-entity quads.</summary>
    public interface IThreadSafeEntityStore : IEntityStore
    {
        /// <summary>Gets or sets a flag indicating whether the <see cref="IEntityStore" /> should work in thread-safe mode.</summary>
        bool ThreadSafe { get; set; }
    }
}