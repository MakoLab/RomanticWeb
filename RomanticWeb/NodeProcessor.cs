using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace RomanticWeb
{
    // todo: consider renaming 
	internal class NodeProcessor:INodeProcessor
	{
	    private readonly IEntityContext _entityContext;

	    private readonly IEntityStore _store;

	    public NodeProcessor(IEntityContext entityContext,IEntityStore store)
	    {
	        _store=store;
	        _entityContext=entityContext;
            Converters=new List<ILiteralNodeConverter>();
            BlankNodeConverters=new List<IBlankNodeConverter>();
	    }
        
        [ImportMany]
        public IEnumerable<ILiteralNodeConverter> Converters { get; internal set; }

        [ImportMany]
        public IEnumerable<IBlankNodeConverter> BlankNodeConverters { get; internal set; }

	    public IEnumerable<object> ProcessNodes(Uri predicate,IEnumerable<Node> objects)
		{
			foreach (var objectNode in objects)
			{
				if (objectNode.IsUri)
				{
                    yield return _entityContext.Create(objectNode.ToEntityId());
				}
                else if (objectNode.IsBlank)
                {
                    yield return ConvertBlankNode(objectNode);
                }
				else
                {
                    yield return ConvertLiteral(objectNode);
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

	    private object ConvertBlankNode(Node objectNode)
	    {
	        var entity=_entityContext.Create(objectNode.ToEntityId());

	        var converter=BlankNodeConverters.FirstOrDefault(c => c.CanConvert(entity,_store));

	        if (converter!=null)
	        {
	            return converter.Convert(entity,_store);
	        }
	        
            return entity;
	    }
	}
}