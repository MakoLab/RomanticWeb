using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    // todo: consider renaming 
	internal class NodeConverter:INodeConverter
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

	    public IEnumerable<object> ConvertNodes(Uri predicate,IEnumerable<Node> objects)
		{
			foreach (var objectNode in objects.ToList())
			{
			    if (objectNode.IsLiteral)
			    {
			        yield return ConvertLiteral(objectNode);
			    }
			    else
			    {
			        yield return ConvertEntity(objectNode);
			    }
			}
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

	    private object ConvertEntity(Node objectNode)
	    {
	        var entity=_entityContext.Load(objectNode.ToEntityId(),false);

	        var converter=ComplexTypeConverters.FirstOrDefault(c => c.CanConvert(entity,_store));

	        if (converter!=null)
	        {
	            return converter.Convert(entity,_store);
	        }
	        
            return entity;
	    }
	}
}