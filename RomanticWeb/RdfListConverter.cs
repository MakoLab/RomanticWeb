using System.Collections.Generic;
using System.ComponentModel.Composition;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    [Export(typeof(IComplexTypeConverter))]
    [NullGuard(ValidationFlags.OutValues)]
    public class RdfListConverter:IComplexTypeConverter
    {
        private readonly Entity _listNil;

        public RdfListConverter()
        {
            _listNil = new Entity(new EntityId("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil"));
        }

        public bool CanConvert(IEntity blankNode,IEntityStore entityStore)
        {
            dynamic potentialList=blankNode.AsDynamic();

            // todo: consider removing EntityIsCollectionRoot
            return potentialList.rdf.first!=null&&entityStore.EntityIsCollectionRoot(potentialList);
        }

        public object Convert(IEntity blankNode, IEntityStore entityStore)
        {
            // todo: consider removing dynamic typing
            dynamic potentialList = blankNode.AsDynamic();
            var list = new List<object>();

            dynamic currentElement = potentialList.rdf.first;
            dynamic currentListNode = potentialList;

            while (currentListNode != _listNil)
            {
                list.Add(currentElement);
                currentListNode = currentListNode.rdf.rest;
                currentElement = currentListNode.rdf.first;
            }

            return list;
        }
    }
}