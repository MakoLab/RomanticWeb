using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>
	/// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
	/// </summary>
	[NullGuard(ValidationFlags.OutValues)]
	public sealed class OntologyAccessor : ImpromptuDictionary, IObjectAccessor
	{
		private readonly ITripleSource _tripleSource;
		private readonly EntityId _entityId;
		private readonly Ontology _ontology;
		private readonly IRdfNodeConverter _nodeConverter;

		/// <summary>
		/// Creates a new instance of <see cref="OntologyAccessor"/>
		/// </summary>
		/// <param name="tripleSource">underlying RDF source</param>
		/// <param name="entity">the access Entity</param>
		/// <param name="ontology">Ontolgy used to resolve predicate names</param>
		/// <param name="entityFactory">factory used to produce associated Entities</param>
		public OntologyAccessor(ITripleSource tripleSource, EntityId entityId, Ontology ontology, IRdfNodeConverter nodeConverter)
		{
			_tripleSource = tripleSource;
			_entityId = entityId;
			_ontology = ontology;
			_nodeConverter = nodeConverter;
		}

		internal IEnumerable<Property> KnownProperties
		{
			get { return _ontology.Properties; }
		}

		/// <summary>
		/// Tries to retrieve subjects from the backing RDF source for a dynamically resolved property
		/// </summary>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var property = KnownProperties.SingleOrDefault(p => p.PropertyName == binder.Name);

			if (property == null)
			{
				property = new Property(binder.Name).InOntology(_ontology);
			}

			result = ((IObjectAccessor)this).GetObjects(_entityId, property);

			return true;
		}

		dynamic IObjectAccessor.GetObjects(EntityId entity, Property predicate)
		{
			var subjectValues = _tripleSource.GetObjectsForPredicate(entity, predicate.Uri);
			var subjects = _nodeConverter.Convert(subjectValues, _tripleSource).ToList();

			if (subjects.Count == 1)
			{
				return subjects.Single();
			}

			if (subjects.Count == 0)
			{
				return null;
			}

			return subjects;
		}
	}
}