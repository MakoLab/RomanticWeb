using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Dynamic
{
    /// <summary>
    /// Contract for implementing classes, which provide
    /// types of generated dictionary owner and entry classes
    /// </summary>
    public interface IDictionaryTypeProvider
    {
        /// <summary>
        /// Gets the type of the dictionary entry.
        /// </summary>
        Type GetEntryType(IPropertyMapping property);

        /// <summary>
        /// Gets the type of the dictionary owner.
        /// </summary>
        Type GetOwnerType(IPropertyMapping property);
    }
}