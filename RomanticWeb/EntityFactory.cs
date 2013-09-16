using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using ImpromptuInterface;

using RomanticWeb.Linq;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>Base class for factories, which produce <see cref="Entity"/> instances.</summary>
	public class EntityFactory : IEntityFactory
	{
		private readonly TripleSourceFactoryBase _sourceFactoryBase;

		private readonly IMappingsRepository _mappings;

	    private readonly IOntologyProvider _ontologyProvider;

	    internal EntityFactory(IMappingsRepository mappings, IOntologyProvider ontologyProvider, TripleSourceFactoryBase sourceFactoryBase)
		{
			_mappings = mappings;
			_sourceFactoryBase = sourceFactoryBase;
			_ontologyProvider = new DefaultOntologiesProvider(ontologyProvider);
		}

		public IQueryable<Entity> AsQueryable()
		{
			return new EntityQueryable<Entity>(this);
		}

		public IQueryable<T> AsQueryable<T>() where T : class,IEntity
		{
			return new EntityQueryable<T>(this);
		}

		public Entity Create(EntityId entityId)
		{
			Entity entity = new Entity(entityId, this);
			IDictionary<string, object> typeCheckerExpando = new ExpandoObject();

			foreach (var ontology in _ontologyProvider.Ontologies)
			{
				var source = _sourceFactoryBase.CreateTriplesSourceForOntology();
				entity[ontology.Prefix] = new OntologyAccessor(source, entityId, ontology, new RdfNodeConverter(this));
				typeCheckerExpando[ontology.Prefix] = new TypeCheckerAccessor(entity, ontology);
			}

			entity["IsA"] = typeCheckerExpando;
			return entity;
		}

		public T Create<T>(EntityId entityId) where T : class,IEntity
		{
		    if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
			{
			    return (T)(IEntity)Create(entityId);
			}
		
            return this.EntityAs<T>(this.Create(entityId));
		}

	    public IEnumerable<Entity> Create(string sparqlConstruct)
		{
			return Create<Entity>(sparqlConstruct);
		}

		public IEnumerable<T> Create<T>(string sparqlConstruct) where T : class,IEntity
		{
			IList<T> entities = new List<T>();

			ITripleSource tripleSource = _sourceFactoryBase.CreateTriplesSourceForOntology();
			IEnumerable<Tuple<RdfNode, RdfNode, RdfNode>> triples = tripleSource.GetNodesForQuery(sparqlConstruct);
			foreach (RdfNode subject in triples.Select(triple => triple.Item1).Distinct(RdfNodeEqualityComparer.Default))
			{
			    entities.Add(Create<T>(EntityId.Create(subject.ToString())));
			}

			return entities;
		}

		internal T EntityAs<T>(IEntity entity) where T : class,IEntity
		{
			return new EntityProxy<T>(_sourceFactoryBase, entity.Id, _mappings.MappingFor<T>(), new RdfNodeConverter(this)).ActLike<T>();
		}
	}
}