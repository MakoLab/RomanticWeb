using System.Collections.Generic;
using System.ComponentModel.Composition;
using RomanticWeb.Entities;

namespace RomanticWeb.Converters
{
    [Export(typeof(IComplexTypeConverter))]
    public class RdfListConverter:IComplexTypeConverter
    {
        private readonly EntityId _listNilId;

        public RdfListConverter()
        {
            _listNilId=new EntityId("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil");
        }

        public bool CanConvert(IEntity blankNode,IEntityStore entityStore)
        {
            return (blankNode.AsDynamic().rdf.Has_first)&&(entityStore.EntityIsCollectionRoot(blankNode));
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