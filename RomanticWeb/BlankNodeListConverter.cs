using System.Collections.Generic;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    [NullGuard(ValidationFlags.OutValues)]
    public class BlankNodeListConverter
    {
        private readonly IEntityStore _tripleSource;

        private readonly Entity _listNil;

        public BlankNodeListConverter(IEntityStore tripleSource)
        {
            _tripleSource=tripleSource;
            _listNil = new Entity(new EntityId("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil"));
        }

        public bool TryConvert(dynamic potentialList,out object actualList)
        {
            if (potentialList.rdf.first != null && _tripleSource.TripleIsCollectionRoot(potentialList))
            {
                var list = new List<object>();

                dynamic currentElement=potentialList.rdf.first;
                dynamic currentListNode=potentialList;

                while (currentListNode!=_listNil)
                {
                    list.Add(currentElement);
                    currentListNode=currentListNode.rdf.rest;
                    currentElement=currentListNode.rdf.first;
                }

                actualList=list;
                return true;
            }

            actualList=null;
            return false;
        }
    }
}