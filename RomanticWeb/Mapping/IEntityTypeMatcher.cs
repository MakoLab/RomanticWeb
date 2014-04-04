using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping
{
    public interface IEntityTypeMatcher
    {
        Type GetMostDerivedMappedType(IEntity entity,Type requestedType);
    }
}