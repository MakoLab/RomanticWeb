using System;

namespace RomanticWeb.DotNetRDF.Configuration.StorageProviders
{
    internal static class Validators
    {
        public static void ValidateType(object typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName.ToString()))
            {
                return;
            }

            if (Type.GetType(typeName.ToString())==null)
            {
                throw new ArgumentException(string.Format("Cannot load type {0}",typeName));
            }
        }
    }
}