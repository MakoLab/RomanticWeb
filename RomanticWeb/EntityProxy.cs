using System;
using System.Collections;
using System.Dynamic;
using System.Linq;

namespace RomanticWeb
{
    using RomanticWeb.Mapping.Model;

    internal class EntityProxy<T>:DynamicObject where T:IEntity
	{
		private readonly TripleSourceFactoryBase _sourceFactory;
		private readonly EntityId _entityId;
		private readonly IMapping _mappings;
		private readonly RdfNodeConverter _converter;

		public EntityProxy(TripleSourceFactoryBase source, EntityId entity, IMapping mappings, RdfNodeConverter converter)
		{
			_sourceFactory=source;
			_entityId=entity;
			_mappings=mappings;
			_converter=converter;
		}

		public override bool TryGetMember(GetMemberBinder binder,out object result)
		{
			var property=_mappings.PropertyFor(binder.Name).Uri;

			ITripleSource source=_sourceFactory.CreateTripleSourceForProperty(_entityId,_mappings.PropertyFor(binder.Name));
			IList objectsForPredicate=_converter.Convert(source.GetObjectsForPredicate(_entityId,property),source).ToList();

			if (objectsForPredicate.Count==1)
			{
			    result = objectsForPredicate[0];
			}
			else if (objectsForPredicate.Count==0)
			{
			    result=null;
			}
			else
			{
			    result = objectsForPredicate;
			}

			return true;
		}
	}
}