using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities
{
    public static class EntityExtensions
    {
        public static IEnumerable<EntityId> GetTypes(this IEntity entity)
        {
            IEnumerable result=(IEnumerable)entity.AsDynamic().rdf.type;
            return (result!=null?result.Cast<EntityId>().ToArray():new EntityId[0]);
        }
    }
}