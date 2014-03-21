using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Dynamic
{
    public interface IDictionaryPairTypeProvider
    {
        Type GetEntryType(IPropertyMapping property);

        Type GetOwnerType(IPropertyMapping property);
    }
}