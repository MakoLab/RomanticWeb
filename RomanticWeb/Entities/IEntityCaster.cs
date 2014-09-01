using System;

namespace RomanticWeb.Entities
{
    internal interface IEntityCaster
    {
        T EntityAs<T>(Entity entity, Type[] types) where T : IEntity;
    }
}