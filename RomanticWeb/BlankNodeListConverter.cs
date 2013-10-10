using System.Collections.Generic;
using System.ComponentModel.Composition;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    [Export(typeof(IBlankNodeConverter))]
    [NullGuard(ValidationFlags.OutValues)]
    public class BlankNodeListConverter:IBlankNodeConverter
    {
        private readonly Entity _listNil;

        public BlankNodeListConverter()
        {
            _listNil = new Entity(new EntityId("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil"));
        }

        public bool CanConvert(IEntity blankNode,IEntityStore entityStore)
        {
            dynamic potentialList=blankNode.AsDynamic();
            return potentialList.rdf.first!=null&&entityStore.EntityIsCollectionRoot(potentialList);
        }

        public object Convert(IEntity blankNode, IEntityStore entityStore)
        {
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