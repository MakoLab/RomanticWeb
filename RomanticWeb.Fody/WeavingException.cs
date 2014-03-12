using System;

namespace RomanticWeb.Fody
{
    public class WeavingException : Exception
    {
        public WeavingException(string message)
            : base(message)
        {
        }
    }
}