namespace RomanticWeb.Entities.ResultAggregations
{
    /// <summary>Contains kinds of operation performed on multiple results when reading predicate values.</summary>
    public enum Aggregation
    {
        /// <summary>Leave results intact.</summary>
        Original,

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