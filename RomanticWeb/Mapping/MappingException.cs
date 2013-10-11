using System;

namespace RomanticWeb.Mapping
{
    public class MappingException : Exception
    {
        internal MappingException(string message)
            : base(message)
        {
        }
    }
}