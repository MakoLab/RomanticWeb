using System;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Represents errors, which occur when an entity type is unmapped
    /// </summary>
    public class UnMappedTypeException:MappingException
    {
        internal UnMappedTypeException(Type type)
            :base(string.Format("Mapping not found for type '{0}'", type))
        {
            Type=type;
        }

        /// <summary>
        /// The type which wasn't found in the mapping repositories
        /// </summary>
        public Type Type { get; private set; }
    }
}