using System;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for rdf class
    /// </summary>
    public interface IClassMap
    {
        /// <summary>
        /// Sets the class name 
        /// </summary>
        IClassMap Is(string prefix, string className);

        /// <summary>
        /// Sets the class name 
        /// </summary>
        IClassMap Is(Uri uri);
    }
}