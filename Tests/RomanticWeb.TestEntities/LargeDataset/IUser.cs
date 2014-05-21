using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using System.Collections.Generic;
using RomanticWeb.TestEntities.LargeDataset;

namespace RomanticWeb.TestEntities.LargeDataset
{
    [Class("schema","Person")]
    public interface IUser:IEntity
    {
        [Collection("chem","favorite")]
        IEnumerable<IProduct> FavoriteProduct { get; set; }

        [Property("chem","group")]
        EntityId Group { get; set; }
    }
}