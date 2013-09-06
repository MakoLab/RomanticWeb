using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public class EntityFactory : EntityFactoryBase<ITripleStore>
    {
        public EntityFactory(IOntologyProvider ontologyProvider) : base(ontologyProvider)
        {
        }

        protected override PredicateAccessor<ITripleStore> CreatePredicateAccessor(Entity entity)
        {
            throw new NotImplementedException();
        }

        protected override Entity CreateInternal(EntityId entityId)
        {
            return new Entity(entityId);
        }
    }
}
