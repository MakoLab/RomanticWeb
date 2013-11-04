using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
	internal sealed class NodeConverter:INodeConverter
	{
	    private readonly IEntityContext _entityContext;

	    private readonly IEntityStore _store;

	    public NodeConverter(IEntityContext entityContext,IEntityStore store)
	    {
	        _store=store;
	        _entityContext=entityContext;
            Converters=new List<ILiteralNodeConverter>();
            ComplexTypeConverters=new List<IComplexTypeConverter>();
	    }
        
        [ImportMany]
        public IEnumerable<ILiteralNodeConverter> Converters { get; internal set; }

        [ImportMany]
        public IEnumerable<IComplexTypeConverter> ComplexTypeConverters { get; internal set; }

	    public IEnumerable<object> ConvertNodes(IEnumerable<Node> objects,IPropertyMapping predicate)
		{
			foreach (var objectNode in objects.ToList())
			{
			    if (objectNode.IsLiteral)
			    {
			        yield return ConvertLiteral(objectNode);
			    }
			    else
			    {
			        yield return ConvertEntity(objectNode,predicate);
			    }
			}
		}

	    public IEnumerable<object> ConvertNodes(IEnumerable<Node> objects)
	    {
	        return ConvertNodes(objects,null);
	    }

        private static bool PredicateIsEntityOrCollectionThereof(IPropertyMapping predicate,out Type entityType)
        {
            entityType=null;
            if (predicate==null)
            {
                return false;
            }

            if (typeof(IEntity).IsAssignableFrom(predicate.ReturnType))
            {
                entityType = predicate.ReturnType;
                return true;
            }
            
            if (typeof(IEnumerable<IEntity>).IsAssignableFrom(predicate.ReturnType))
            {
                entityType=predicate.ReturnType.GenericTypeArguments.Single();
                return true;
            }

            return false;
        }

	    private object ConvertLiteral(Node objectNode)
	    {
	        if (objectNode.DataType==null)
	        {
	            return objectNode.Literal;
	        }
	        
            var converter=Converters.FirstOrDefault(c => c.CanConvert(objectNode.DataType));
	        if (converter!=null)
	        {
	            return converter.Convert(objectNode);
	        }

	        return objectNode.Literal;
	    }

	    private object ConvertEntity(Node objectNode,IPropertyMapping predicate)
	    {
	        var entity=_entityContext.Load<IEntity>(objectNode.ToEntityId(),false);

	        var converter=ComplexTypeConverters.FirstOrDefault(c => c.CanConvert(entity,_store));

	        if (converter!=null)
	        {
	            return converter.Convert(entity,_store);
	        }

	        Type entityType;
            if (PredicateIsEntityOrCollectionThereof(predicate, out entityType))
            {
                var wrapEntity = Info.OfMethod("RomanticWeb", "RomanticWeb.Entities.Entity", "AsEntity")
                                     .MakeGenericMethod(entityType);

                return wrapEntity.Invoke(entity,null);
            }
	        
            return entity;
	    }
	}
}