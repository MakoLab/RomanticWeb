namespace RomanticWeb.Entities.ResultAggregations
{
    /// <summary>Contains kinds of operation performed on multiple results when reading predicate values.</summary>
    public enum ProcessingOperation
    {
        /// <summary>Leave results as-if.</summary>
        Original,

        /// <summary>Combine all values into a single dictionary.</summary>
        /// <remarks>This operation should also perform a flattening operation as well.</remarks>
        Dictionary,

        /// <summary>Combine all values into a single collection.</summary>
        Flatten,

        /// <summary>Return only a single value.</summary>
        Single,

        /// <summary>Return a single value or null.</summary>
        SingleOrDefault,

        /// <summary>Return the first value.</summary>
        First,

        /// <summary>Return the first value or null.</summary>
        FirstOrDefault,

        /// <summary>Check if any result exists.</summary>
        Has
    }
}