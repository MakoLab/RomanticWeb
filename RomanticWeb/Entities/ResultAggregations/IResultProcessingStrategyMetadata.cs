namespace RomanticWeb.Entities.ResultAggregations
{
    /// <summary>
    /// Represents metadata about a <see cref="IResultProcessingStrategy"/>
    /// </summary>
    public interface IResultProcessingStrategyMetadata
    {
        /// <summary>
        /// The <see cref="ProcessingOperation"/> performed by the described <see cref="IResultProcessingStrategy"/>
        /// </summary>
        ProcessingOperation Operation { get; }
    }
}