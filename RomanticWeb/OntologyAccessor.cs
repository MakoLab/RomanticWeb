using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>
	/// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
	/// </summary>
	/// todo: make a DynamicObject
	[NullGuard(ValidationFlags.OutValues)]
	internal sealed class OntologyAccessor : ImpromptuDictionary
	{
		private readonly IEntityStore _tripleSource;
		private readonly Entity _entity;
		private readonly Ontology _ontology;
		private readonly INodeProcessor _nodeProcessor;

		/// <summary>
		/// Creates a new instance of <see cref="OntologyAccessor"/>
		/// </summary>
		/// <param name="tripleSource">underlying RDF source</param>
		/// <param name="entity">the access Entity</param>
		/// <param name="ontology">Ontolgy used to resolve predicate names</param>
		internal OntologyAccessor(IEntityStore tripleSource, Entity entity, Ontology ontology, INodeProcessor nodeProcessor)
		{
			_tripleSource = tripleSource;
			_entity = entity;
			_ontology = ontology;
			_nodeProcessor = nodeProcessor;
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
				property = new Property(binder.Name).InOntology(_ontology);
			}

			result = GetObjects(_entity.Id, property);

			return true;
		}

	    internal dynamic GetObjects(EntityId entity, Property predicate)
		{
			var objectValues = _tripleSource.GetObjectsForPredicate(entity, predicate.Uri);
			var objects = _nodeProcessor.ProcessNodes(predicate.Uri,objectValues).ToList();

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