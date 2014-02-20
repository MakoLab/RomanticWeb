using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.JsonLd
{
    public partial class JsonLdProcessor:IJsonLdProcessor
    {
        private const string Id="@id";
        private const string Language="@language";
        private const string Value="@value";
        private const string Vocab="@vocab";
        private const string Context="@context";
        private const string Graph="@graph";
        private const string Type="@type";
        private const string List="@list";
        private const string Reverse="@reverse";
        private const string Container="@container";
        private const string Set="@set";
        private const string Index="@index";
        private const string Base="@base";

        private readonly JArray _listInGraph=new JArray();
        private List<Node> _nodesInList=new List<Node>();

        public string FromRdf(IEnumerable<EntityQuad> quads,bool userRdfType=false,bool useNativeTypes=false)
        {
            IDictionary<JToken,JObject> subjectMap=new Dictionary<JToken,JObject>();
            var dataset=from triple in quads
                        group triple by triple.Graph into g
                        select g;

            foreach (var graph in dataset)
            {
                var lists=GetListsFromGraph(graph,useNativeTypes);
                IGrouping<Node,EntityQuad> graph1=graph;
                var distinctSubjects=(from q in graph
                                      where q.Graph==graph1.Key&&(!_nodesInList.Contains(q.Subject))
                                      orderby q.Subject.IsBlank?"_:"+q.Subject.BlankNode:q.Subject.ToString() // todo: remove ordering (will need to change tests)
                                      select q.Subject).Distinct();

                foreach (var subject in distinctSubjects)
                {
                    Node subject1=subject;
                    var entitySerialized=SerializeEntity(subject,graph.Where(n => n.Subject==subject1),graph.Key,useNativeTypes,userRdfType,lists);

                    if (graph.Key!=null)
                    {
                        var namedGraph=new JObject();
                        namedGraph[Id]=graph.Key.ToString();
                        namedGraph[Graph]=new JArray(entitySerialized);
                        entitySerialized=namedGraph;
                    }

                    if (subjectMap.ContainsKey(entitySerialized[Id]))
                    {
                        subjectMap[entitySerialized[Id]].Merge(entitySerialized);
                    }
                    else
                    {
                        subjectMap[entitySerialized[Id]]=entitySerialized;
                    }
                }
            }

            return new JArray(subjectMap.Values).ToString();
        }

        public IEnumerable<EntityQuad> ToRdf(string json)
        {
            yield break;
        }

        public string Flatten(string json,string jsonLdContext)
        {
            return json;
        }

        public string Compact(string json,string jsonLdContext)
        {
            return json;
        }

        private JObject SerializeEntity(Node subject,IEnumerable<EntityQuad> quads,Node graphName,bool nativeTypes,bool useRdfType,JObject listsInGraph)
        {
            var groups=from quad in quads
                       where quad.Subject==subject&&quad.Graph==graphName
                       group quad.Object by quad.Predicate into g
                       select new
                       {
                           Predicate=(g.Key==Node.a?useRdfType?Node.a:Node.ForLiteral(Type):g.Key),
                           Objects=g
                       }
                           into selection
                           orderby selection.Predicate
                           select selection;

            var result=new JObject();
            int i=0;

            foreach (var objectGroup in groups)
            {
                JProperty res;
                if (objectGroup.Predicate==Node.a||objectGroup.Predicate==Node.ForLiteral(Type))
                {
                    if (useRdfType)
                    {
                        res=new JProperty(new JProperty(Node.a.ToString(),new JArray(from o in objectGroup.Objects select GetPropertyValue(o,nativeTypes,listsInGraph))));
                    }
                    else
                    {
                        res=new JProperty(new JProperty(Type,new JArray(from o in objectGroup.Objects select o.ToString())));
                    }
                }
                else
                {
                    res=new JProperty(new JProperty(objectGroup.Predicate.Uri.ToString(),new JArray(from o in objectGroup.Objects select GetPropertyValue(o,nativeTypes,listsInGraph))));
                }

                if (i==0)
                {
                    result.AddFirst(new JProperty(Id,subject.IsBlank?"_:"+subject.BlankNode:subject.ToString()));
                    i++;
                }

                result.Add(res);
            }

            return result;
        }

        private JObject GetListsFromGraph(IEnumerable<EntityQuad> quads,bool useNativeTypes)
        {
            var lists=quads.Where(q => (q.Subject.IsBlank&&q.Predicate==Node.rest&&q.Object==Node.nil));
            var locList=new JObject();
            foreach (var list in lists)
            {
                _listInGraph.Clear();
                PrepareLists(list,quads,useNativeTypes);
                string indexer=_listInGraph.First().ToString();
                _listInGraph.First.Remove();
                locList.Add(new JProperty(indexer,new JArray(_listInGraph)));
                _listInGraph.Clear();
            }

            return locList;
        }

        private void PrepareLists(EntityQuad list,IEnumerable<EntityQuad> quads,bool useNativeTypes)
        {
            Node localSubject=list.Subject;
            int anotherPropertyInList=quads.Count(q => q.Subject==localSubject&&q.Predicate!=Node.a);
            var firstValues=quads.Where(q => (q.Subject==localSubject&&q.Predicate==Node.first)).Select(q => q.Object);
            Node firstValue=firstValues.First();
            Node restValue=list.Object;
            _listInGraph.AddFirst(ReturnListProperties(firstValue,useNativeTypes));
            _nodesInList.Add(localSubject);

            IEnumerable<EntityQuad> quad=quads.Where(q => (q.Subject.IsBlank&&q.Predicate==Node.rest&&q.Object==localSubject));
            if (anotherPropertyInList>2||firstValues.Count()>1)
            {
                if (_listInGraph.Count()>1)
                {
                    _listInGraph.First.Remove();
                    _nodesInList=_nodesInList.Where(n => n!=localSubject).ToList();
                }

                _listInGraph.AddFirst(restValue.ToString());
            }
            else
                if (quad.Count()==1)
                {
                    PrepareLists(quad.First(),quads,useNativeTypes);
                }
                else
                {
                    IEnumerable<EntityQuad> firstQuad=quads.Where(q => (q.Subject.IsBlank&&q.Predicate==Node.first&&q.Object==localSubject));
                    if (firstQuad.Count()==1)
                    {
                        _listInGraph.First.Remove();
                        _nodesInList=_nodesInList.Where(n => n!=localSubject).ToList();
                        _listInGraph.AddFirst((restValue.IsBlank&&firstValue.IsBlank)?localSubject.ToString():restValue.ToString());
                    }
                    else
                    {
                        _listInGraph.AddFirst(localSubject.ToString());
                    }
                }
        }

        private JObject ReturnListProperties(Node @object,bool useNativeTypes)
        {
            var returnObject=new JObject();
            if (!@object.IsLiteral)
            {
                var id=new JProperty(Id,@object.IsBlank?"_:"+@object.BlankNode:@object.ToString());
                returnObject.Add(id);
            }
            else
            {
                returnObject.Add(GetLiteralObjectProperties(@object,useNativeTypes));
            }

            return returnObject;
        }

        private JObject GetPropertyValue(Node @object,bool nativeTypes,JObject lists)
        {
            if (!@object.IsLiteral)
            {
                return new JObject(GetNonLiteralObjectPreoperties(@object,lists));
            }

            return new JObject(GetLiteralObjectProperties(@object,nativeTypes));
        }

        private JProperty GetNonLiteralObjectPreoperties(Node @object,JObject lists)
        {
            if (@object==Node.nil)
            {
                return new JProperty(List,new JArray());
            }

            if (lists.Property(@object.ToString())!=null)
            {
                var localList=lists.Property(@object.ToString());
                if (localList!=null)
                {
                    return new JProperty(List,localList.Value);
                }
            }
            else
            {
                return new JProperty(Id,@object.IsBlank?"_:"+@object.BlankNode:@object.ToString());
            }

            return null;
        }

        private IEnumerable<JProperty> GetLiteralObjectProperties(Node @object,bool useNativeTypes)
        {
            IList<JProperty> literalProperties=new List<JProperty>(3);
            object value=null;
            Uri dataType=null;

            if (@object.DataType!=null)
            {
                dataType=@object.DataType;
                if (useNativeTypes)
                {
                    value=TryConvertValueToNative(@object);
                    if (value!=null)
                    {
                        dataType=null;
                    }
                }
            }

            if (value==null)
            {
                value=@object.ToString();
            }

            literalProperties.Add(new JProperty(Value,value));
            if (dataType!=null)
            {
                literalProperties.Add(new JProperty(Type,dataType.ToString()));
            }

            if (@object.Language!=null)
            {
                literalProperties.Add(new JProperty(Language,@object.Language));
            }

            return literalProperties;
        }

        private object TryConvertValueToNative(Node @object)
        {
            object contverted=null;

            if (AbsoluteUriComparer.Default.Compare(@object.DataType,Xsd.Boolean)==0)
            {
                contverted=Convert.ToBoolean(@object.Literal);
            }
            else if (AbsoluteUriComparer.Default.Compare(@object.DataType,Xsd.Integer)==0)
            {
                contverted=Convert.ToInt64(@object.Literal);
            }
            else if (AbsoluteUriComparer.Default.Compare(@object.DataType,Xsd.Double)==0)
            {
                double doubleVal;
                double.TryParse(@object.Literal,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out doubleVal);
                contverted=doubleVal;
            }

            return contverted;
        }
    }
}