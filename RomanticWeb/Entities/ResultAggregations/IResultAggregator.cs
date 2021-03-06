﻿using System.Collections.Generic;

namespace RomanticWeb.Entities.ResultAggregations
{
    /// <summary>
    /// Defines a contract for processing results from reading rdf nodes
    /// </summary>
    public interface IResultAggregator
    {
        /// <summary>
        /// Gets the aggregation.
        /// </summary>
        /// <value>
        /// The aggregation.
        /// </value>
        Aggregation Aggregation { get; }

        /// <summary>
        /// Processes nodes and returns the transformed value
        /// </summary>
        object Aggregate(IEnumerable<object> objects);
    }
}