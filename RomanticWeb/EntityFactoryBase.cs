using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public abstract class EntityFactoryBase<TTripleSource> : IEntityFactory
    {
        private readonly IOntologyProvider _ontologyProvider;

        protected EntityFactoryBase(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider = new DefaultOntologiesProvider(ontologyProvider);
        }

        public Entity Create(EntityId entityId)
        {
            Entity entity = CreateInternal(entityId);
            IDictionary<string, object> typeCheckerExpando = new ExpandoObject();

            foreach (var ontology in _ontologyProvider.Ontologies)
            {
                entity[ontology.Prefix] = CreatePredicateAccessor(entity, ontology);
                typeCheckerExpando[ontology.Prefix] = new TypeCheckerAccessor(entity, ontology);
            }
            entity["IsA"] = typeCheckerExpando;

            return entity;
        }

        private static dynamic CreateTypeCheckerAccessor(dynamic entity, Ontology ontology)
        {
            IDictionary<string, object> typeCheckAccessor = new ExpandoObject();

            foreach (var rdfType in ontology.Classes)
            {
                typeCheckAccessor[rdfType.ClassName] = new Func<bool>(() => entity.rdf.Type != null);
            }

            return typeCheckAccessor;
        }

        protected abstract PredicateAccessor<TTripleSource> CreatePredicateAccessor(Entity entity, Ontology ontology);

        protected abstract Entity CreateInternal(EntityId entityId);
    }

    internal class TypeCheckerAccessor : DynamicObject
    {
        private readonly dynamic _entity;
        private readonly Ontology _ontology;

        public TypeCheckerAccessor(dynamic entity, Ontology ontology)
        {
            _entity = entity;
            _ontology = ontology;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var rdfClass = _ontology.Classes.SingleOrDefault(c => c.ClassName == binder.Name);

            if (rdfClass == null)
            {
                throw new UnknownClassException(_ontology.BaseUri, binder.Name);
            }

            IEnumerable<object> types = _entity.rdf.type;
            result = types.OfType<Entity>().Any(t => t.Id == new EntityId(_ontology.ResolveUri(binder.Name)));

            return true;
        }
    }

    internal class UnknownClassException : Exception
    {
        public UnknownClassException(Uri ontologyUri, string className)
            : base(string.Format("Unknown rdf class '{0}'", new Uri(ontologyUri + className)))
        {
        }
    }
}