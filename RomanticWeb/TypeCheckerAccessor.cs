using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
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
                rdfClass = new RdfClass(binder.Name).InOntology(_ontology);
            }

            IEnumerable<object> types = _entity.rdf.type;
            result = types.OfType<Entity>().Any(t => Equals(t.Id, new UriId(rdfClass.Uri)));

            return true;
        }
    }
}