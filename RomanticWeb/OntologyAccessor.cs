using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>
	/// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
	/// </summary>
	[NullGuard(ValidationFlags.OutValues)]
	public sealed class OntologyAccessor : ImpromptuDictionary
	{
		private readonly IEntityStore _tripleSource;
		private readonly Entity _entity;
		private readonly Ontology _ontology;
		private readonly IRdfNodeConverter _nodeConverter;

		/// <summary>
		/// Creates a new instance of <see cref="OntologyAccessor"/>
		/// </summary>
		/// <param name="tripleSource">underlying RDF source</param>
		/// <param name="entity">the access Entity</param>
		/// <param name="ontology">Ontolgy used to resolve predicate names</param>
		internal OntologyAccessor(IEntityStore tripleSource, Entity entity, Ontology ontology, IRdfNodeConverter nodeConverter)
		{
			_tripleSource = tripleSource;
			_entity = entity;
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
            _entity.EnsureIsInitialized();

			var property = KnownProperties.SingleOrDefault(p => p.PropertyName == binder.Name);

			if (property == null)
            {
                LogTo.Debug("Predicate {0} not found. Creating one impliclty from name", binder.Name);
				property = new Property(binder.Name).InOntology(_ontology);
			}

			result = GetObjects(_entity.Id, property);

			return true;
		}

	    internal dynamic GetObjects(EntityId entity, Property predicate)
		{
			var objectValues = _tripleSource.GetObjectsForPredicate(entity, predicate.Uri).ToList();
			var objects = _nodeConverter.Convert(objectValues, _tripleSource).ToList();

			if (objects.Count == 1)
			{
				return objects.Single();
			}

			if (objects.Count == 0)
			{
				return null;
			}

			return objects;
		}
	}
}