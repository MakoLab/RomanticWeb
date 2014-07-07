using System.Configuration;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// Declaratively instructs the .NET Framework to perform Uri validation on a configuration property. This class cannot be inherited.
    /// </summary>
    public sealed class UriValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriValidatorAttribute"/> class.
        /// </summary>
        public UriValidatorAttribute()
            : base(typeof(UriValidator))
        {
        }
    }
}