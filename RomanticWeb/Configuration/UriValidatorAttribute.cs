using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class UriValidatorAttribute:ConfigurationValidatorAttribute
    {
        public UriValidatorAttribute()
            :base(typeof(UriValidator))
        {
        }
    }
}