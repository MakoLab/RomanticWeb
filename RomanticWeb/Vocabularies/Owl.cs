using System;

namespace RomanticWeb.Vocabularies
{
    internal static class Owl
    {
        public const string BaseUri="http://www.w3.org/2002/07/owl#";

        public static readonly Uri Thing=new Uri(BaseUri+"Thing");
    }
}