using System.Collections.Generic;
using System.ComponentModel.Composition;
using RomanticWeb.Entities;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts a RDF list to a collection
    /// </summary>
    [Export(typeof(IComplexTypeConverter))]
    public class RdfListConverter:IComplexTypeConverter
    {
        private readonly EntityId _listNilId;

        /// <summary>
        /// Create a new instance of <see cref="RdfListConverter"/>
        /// </summary>
        public RdfListConverter()
        {
            _listNilId=new EntityId("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil");
        }

        public bool CanConvert(IEntity objectNode,IEntityStore entityStore)
        {
            return (objectNode.AsDynamic().rdf.Has_first)&&(entityStore.EntityIsCollectionRoot(objectNode));
        }

        public object Convert(IEntity blankNode,IEntityStore entityStore)
        {
            // todo: consider removing dynamic typing
            dynamic potentialList=blankNode.AsDynamic();
            var list=new List<object>();

            dynamic currentElement=potentialList.rdf.SingleOrDefault_first;
            dynamic currentListNode=potentialList;

            while (currentListNode.Id!=_listNilId)
            {
                list.Add(currentElement);
                currentListNode=currentListNode.rdf.SingleOrDefault_rest;
                if (currentListNode!=null)
                {
                    currentElement=currentListNode.rdf.SingleOrDefault_first;
                }
            }

            return list;
        }
    }
}