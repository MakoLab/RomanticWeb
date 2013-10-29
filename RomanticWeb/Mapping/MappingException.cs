using System;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Represents error with entity mapping
    /// </summary>
    public class MappingException : Exception
    {
        internal MappingException(string message)
            : base(message)
        {
        }
    }
}