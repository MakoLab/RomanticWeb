using System;
using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class UriValidator:ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type == typeof(Uri);
        }

        public override void Validate(object value)
        {
            var uri=value as Uri;

            if (uri != null && !uri.IsAbsoluteUri)
            {
                throw new ArgumentException("Ontology must be a valid absolute URI");
            }
        }
    }
}