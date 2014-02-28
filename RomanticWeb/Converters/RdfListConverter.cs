////using System.Collections;
////using System.Collections.Generic;
////using NullGuard;
////using RomanticWeb.Entities;
////using RomanticWeb.Mapping.Model;
////using RomanticWeb.Model;

////namespace RomanticWeb.Converters
////{
////    /// <summary>Converts a RDF list to a collection.</summary>
////    public class RdfListConverter:IComplexTypeConverter
////    {
////        private readonly EntityId _listNilId;

////        /// <summary>Create a new instance of <see cref="RdfListConverter"/>.</summary>
////        public RdfListConverter()
////        {
////            _listNilId=new EntityId("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil");
////        }

////        /// <summary>Check whether the node can be converted to an rdf:List, ie. whether it is a root of such a list.</summary>
////        /// <param name="objectNode">The root node of the structure</param>
////        /// <param name="entityStore">Store, from which relevant additional nodes are read</param>
////        /// <param name="predicate">Predicate for this node.</param>
////        public bool CanConvert(IEntity objectNode, IEntityStore entityStore, [AllowNull] IPropertyMapping predicate)
////        {
////            return (objectNode.AsDynamic().rdf.Has_first)&&(entityStore.EntityIsCollectionRoot(objectNode));
////        }

////        /// <summary>Converts a collection to an rdf:List triples.</summary>
////        /// <param name="obj">Object to be converted.</param>
////        public IEnumerable<Node> ConvertBack(object obj)
////        {
////            throw new System.NotImplementedException();
////        }

////        /// <summary>Check whether the <paramref name="value"/> is a <see cref="IEnumerable"/> and the property mapping is set to produce rdf:Lists.</summary>
////        /// <param name="value">Value to be checked.</param>
////        /// <param name="predicate">Property mapping for this value.</param>
////        public bool CanConvertBack(object value,IPropertyMapping predicate)
////        {
////            return (value is IEnumerable)&&(predicate.StorageStrategy==StorageStrategyOption.RdfList);
////        }

////        /// <summary>Converts an rdf:List subgraph to a collection.</summary>
////        /// <param name="blankNode">The root node of the structure</param>
////        /// <param name="entityStore">Store, from which relevant additional nodes are read</param>
////        /// <param name="predicate">Predicate for this node.</param>
////        public object Convert(IEntity blankNode, IEntityStore entityStore, [AllowNull] IPropertyMapping predicate)
////        {
////            // todo: consider removing dynamic typing
////            dynamic potentialList=blankNode.AsDynamic();
////            var list=new List<object>();

////            dynamic currentElement=potentialList.rdf.SingleOrDefault_first;
////            dynamic currentListNode=potentialList;

////            while (currentListNode.Id!=_listNilId)
////            {
////                list.Add(currentElement);

////                currentListNode=currentListNode.rdf.SingleOrDefault_rest;
////                if (currentListNode!=null)
////                {
////                    currentElement=currentListNode.rdf.SingleOrDefault_first;
////                }
////            }

////            return list;
////        }
////    }
////}