using System;
using System.Collections;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>Marker interface. The generic should be used to implement conventions.</summary>
    public interface IConvention
    {
        /// <summary>Gets the convention types that are required to be executed before this convention.</summary>
        IEnumerable<Type> Requires { get; }
    }

    /// <summary>A base constract for implementing conventions.</summary>
    /// <typeparam name="T">Type this convention should be applied to.</typeparam>
    public interface IConvention<in T> : IConvention
    {
        /// <summary>Checks if convention should be applied.</summary>
        bool ShouldApply(T target);

        /// <summary>Applies the convention to target.</summary>
        void Apply(T target);
    }
}