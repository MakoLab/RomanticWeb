using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    public interface IDictionaryPairTypeProvider
    {
        Type GetEntryType(IPropertyMapping property);

        Type GetOwnerType(IEntityMapping entity);
    }
}