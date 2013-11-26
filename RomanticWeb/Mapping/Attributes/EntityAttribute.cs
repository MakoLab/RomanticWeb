using System;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>
    /// Marker interface for mapped entities
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class EntityAttribute:Attribute
    {
    }
}