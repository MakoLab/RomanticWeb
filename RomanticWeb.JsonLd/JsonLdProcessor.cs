using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.JsonLd
{
    public class JsonLdProcessor:IJsonLdProcessor
    {
        internal const string Id = "@id";
        internal const string Language = "@language";
        internal const string Value = "@value";
        internal const string Context = "@context";
        internal const string Graph = "@graph";
        internal const string Type = "@type";
        internal const string List = "@list";
        private Node nodeType = Node.ForLiteral("@type");
        private Node rdfType;
        private Node type = Node.ForUri(Rdf.type);
        private Node first = Node.ForUri(Rdf.first);
        private Node rest = Node.ForUri(Rdf.rest);
        private Node nil = Node.ForUri(Rdf.nil);
        private IEnumerable<Node> _distinctGrafs;
        private List<Node> listToInitiation = new List<Node>();
        private JArray listInGraph = new JArray();
        private bool nativeTypes = false;
        private Dictionary<Node, JObject> resLists = new Dictionary<Node, JObject>();
        private List<Node> nodesInList = new List<Node>();

        public string FromRdf(IEnumerable<EntityQuad> dataset,bool userRdfType=false,bool useNativeTypes=false)
        {
            nativeTypes = useNativeTypes;
            if (!userRdfType)
            {
                rdfType = nodeType;
            }
            else
            {
                rdfType = type;
            }

            return GetJsonStructure(dataset).ToString();
        }

        public IEnumerable<EntityQuad> ToRdf(string json)
        {
            yield break;
        }
 
        public string Flatten(string json,string jsonLdContext)
        {
            return json;
        }

        public string Compact(string json, string jsonLdContext)
        {
            return json;
        }

        public string Expand(string json)
        {
            return json;
        }

        private JArray GetJsonStructure(IEnumerable<EntityQuad> quads)
        {
            _distinctGrafs = quads.Select(x => x.Graph).Distinct();
            foreach (var dGraph in _distinctGrafs)
                {
                    GetListsFromGraph(quads, dGraph);
                }

            _distinctGrafs = _distinctGrafs.Where(g => g != null);

            var root = new JArray();
            var context = new JObject();
            var distinctSubjects = quads.Where(q => (q.Graph == null && (!nodesInList.Contains(q.Subject))))
                         .OrderBy(q => q.Subject.IsBlank ? "_:" + q.Subject.BlankNode.ToString() : q.Subject.ToString()).Select(q => q.Subject).Distinct();
            var serialized = distinctSubjects.Select(subject => SerializeEntity(subject, context, quads, null)).ToList();

            if (listToInitiation.Count() > 0)
            {
                List<Node> localListToInitiation = new List<Node>();
                localListToInitiation.AddRange(listToInitiation.OrderBy(o=>o.ToString()).Distinct());
                var localSerialized = serialized;
                serialized = localListToInitiation.Select(o => SerializeEntity(o, context, quads, null)).ToList();
                serialized.AddRange(localSerialized);
            }

            root = new JArray(serialized);
            if (_distinctGrafs.Count() > 0)
            {
                foreach (var dGraph in _distinctGrafs)
                {
                    var graphDistinctSubjects = quads.Where(q => q.Graph == dGraph && q.Predicate != first && q.Predicate != rest)
                                    .OrderBy(q => q.Subject.IsBlank ? "_:" + q.Subject.BlankNode.ToString() : q.Subject.ToString()).Select(q => q.Subject).Distinct();
                    var graphSerialized = graphDistinctSubjects.Select(subject => SerializeEntity(subject, context, quads, dGraph)).ToList();
                    JObject resultGraph = new JObject(new JProperty(Id,dGraph.ToString()),new JProperty(Graph, graphSerialized));
                    root.Add(resultGraph);
                }
            }
            
            return root;        
        }

        private JObject SerializeEntity(Node subject, JObject context, IEnumerable<EntityQuad> quads, Node graphName)
        {
            Node serializeGraph = graphName != null ? graphName : Node.ForLiteral("default");
            JObject listsInGraph = new JObject(resLists[serializeGraph]);
            var groups = from quad in quads
                         where quad.Subject == subject && quad.Graph == graphName
                         group quad.Object by quad.Predicate into g
                         select new
                         {
                             Predicate =(g.Key == type ? rdfType : g.Key),
                             Objects = g
                         }
                         into selection
                         orderby selection.Predicate
                         select selection;

            JObject result = new JObject();
            int i=0;
                        
            foreach (var g in groups)
            {
                JProperty res = g.Predicate.ToString().Replace("\"", String.Empty) == Type ?
                            new JProperty(
                                new JProperty(
                                            rdfType.ToString().Replace("\"", String.Empty),
                                             new JArray(
                                                from o in g.Objects
                                                select o.ToString())))
                :
                            new JProperty(
                                new JProperty(
                                              g.Predicate.ToString(),
                                              new JArray(
                                                     from o in g.Objects
                                                     select ReturnProperties(true, o, graphName, quads, listsInGraph))));
                if (i==0)   
                {
                    result.AddFirst(new JProperty(Id, subject.IsBlank ? "_:" + subject.BlankNode.ToString() : subject.ToString()));
                    i++;
                }

                result.Add(res);
            }

            if (graphName == null)
            {
                if (_distinctGrafs.Where(gr => gr.ToString() == subject.ToString()).Count() > 0)
                {
                    List<JObject> localSerialized = new List<JObject>();
                    var localDistinctSubjects = quads.Where(gr => gr.Graph == subject && gr.Predicate != first && gr.Predicate != rest)
                                           .OrderBy(x => x.Subject.IsBlank ? "_:" + x.Subject.BlankNode.ToString(): x.Subject.ToString()).Select(x => x.Subject).Distinct();
                    localSerialized = localDistinctSubjects.Select(sub => SerializeEntity(sub, context, quads, subject)).ToList();
                    JProperty graph = new JProperty(Graph, localSerialized);
                    result.Add(graph);
                    _distinctGrafs=_distinctGrafs.Where(g => g.ToString() != subject.ToString());
                }
            }

        return result;
        }

        private void GetListsFromGraph(IEnumerable<EntityQuad> quads, Node graph)
        {
            var lists = quads.Where(q => (q.Graph == graph && q.Subject.IsBlank && q.Predicate == rest && q.Object == nil));
            Node localGraph = graph != null ? graph : Node.ForLiteral("default");
            JObject locList = new JObject();
            foreach (var list in lists)
            {
                    listInGraph.Clear();
                    PrepareLists(list, quads);
                    string indexer = listInGraph.First().ToString();
                    listInGraph.First.Remove();
                    locList.Add(new JProperty(indexer, new JArray(listInGraph)));
                    listInGraph.Clear();
             }

                resLists[localGraph] = locList;
         }
        
        private void PrepareLists(EntityQuad list, IEnumerable<EntityQuad> quads)
        {
             JObject result = new JObject();
             Node localSubject = list.Subject;
             int anotherPropertyInList = quads.Where(q => q.Subject == localSubject && q.Predicate != type).Count();
             var firstValues = quads.Where(q => (q.Subject == localSubject && q.Predicate == first)).Select(q => q.Object);
             Node firstValue = firstValues.First();
             Node restValue = list.Object;
             listInGraph.AddFirst(ReturnListProperties(firstValue));
             nodesInList.Add(localSubject);
             
             IEnumerable<EntityQuad> quad = quads.Where(q => (q.Subject.IsBlank && q.Predicate == rest && q.Object == localSubject));
                 if (anotherPropertyInList > 2 || firstValues.Count()>1)
                 {
                     if (listInGraph.Count() > 1)
                     {
                         listInGraph.First.Remove();
                         nodesInList = nodesInList.Where(n => n != localSubject).ToList();
                     }

                     listInGraph.AddFirst(restValue.ToString());
                 }
                 else
                 if (quad.Count() == 1)
                 {
                     PrepareLists(quad.First(), quads);
                 }
                 else
                 {
                     IEnumerable<EntityQuad> firstQuad = quads.Where(q => (q.Subject.IsBlank && q.Predicate == first && q.Object == localSubject));
                     if (firstQuad.Count() == 1)
                     {
                         listInGraph.First.Remove();
                         nodesInList = nodesInList.Where(n => n != localSubject).ToList();
                         listInGraph.AddFirst((restValue.IsBlank && firstValue.IsBlank) ? localSubject.ToString() : restValue.ToString());
                     }
                     else
                     {
                         listInGraph.AddFirst(localSubject.ToString());
                     }
                 }
        }

        private List<Node> GetList(Node Graph, IEnumerable<EntityQuad> quads, IEnumerable<EntityQuad> localQuad)
        {
            List<Node> resultObject = new List<Node>();
            int isBlank=0;

            if (localQuad.Where(q => q.Predicate == first).Count() > 0)
            {
                Node firstObject = localQuad.Where(q => q.Predicate == first).Select(q => q.Object).First();
                resultObject.Add(firstObject);
                if (firstObject.IsBlank)
                {
                    isBlank++;
                }
            }

            if (localQuad.Where(q => q.Predicate == rest).Count() > 0)
            {
                Node restObject = localQuad.Where(q => q.Predicate == rest).Select(q => q.Object).First();
                if (restObject != nil)
                {
                    var restQuads = quads.Where(q => q.Subject == restObject && q.Graph == Graph && (q.Predicate == first || q.Predicate == rest));
                    if (restQuads.Count() > 0)
                    {
                        resultObject.AddRange(GetList(Graph, quads, restQuads));
                    }
                }

                if (restObject.IsBlank)
                {
                    isBlank++;
                }
            }

            if (isBlank==2)
            {
                List<Node> list = new List<Node>(resultObject);
                listToInitiation.AddRange(list.Where(o => o.IsBlank));
            } 
                                
            return resultObject;
        }

        private JObject ReturnListProperties(Node Object)
        {
            JObject returnObject = new JObject();
            if (!Object.IsLiteral)
            {
                JProperty id = new JProperty(Id, Object.IsBlank ? "_:" + Object.BlankNode.ToString() : Object.ToString());
                returnObject.Add(id);
            }
            else
            {
                JProperty value = new JProperty(Value, Object.ToString().Replace("\"", String.Empty));
                returnObject.Add(value);

                if (Object.Language != null)
                {
                    JProperty language = new JProperty(Language, Object.Language.ToString().Replace("\"", String.Empty));
                    returnObject.Add(language);
                }

                if (Object.DataType != null)
                {
                    JProperty localType = new JProperty(Type, Object.DataType.ToString().Replace("\"", String.Empty));
                    returnObject.Add(localType);
                }
            } 

            return returnObject;
        }

        private JObject ReturnProperties(bool NestedList, Node Object, Node Graph, IEnumerable<EntityQuad> quads, JObject lists)
        {
            Graph = Graph != null ? Graph : Node.ForLiteral("default");
            JObject returnObject = new JObject();
            if (!Object.IsLiteral)
                {
                    if (Object == nil)
                    {
                        JProperty listProperty = new JProperty(List, new JArray());
                        returnObject.Add(listProperty);
                    }
                    else
                    {
                        if (lists.Property(Object.ToString()) != null)
                        {
                            var localList = resLists[Graph].Property(Object.ToString());
                            if ((object)localList != null)
                            {
                                returnObject.Add(new JProperty(List, localList.Value));
                            }
                        }
                        else
                        {
                            JProperty id = new JProperty(Id, Object.IsBlank ? "_:" + Object.BlankNode.ToString() : Object.ToString());
                            returnObject.Add(id);
                        }
                    }
                }
                else
                {
                    if (!nativeTypes)
                    {
                        JProperty value = new JProperty(Value, Object.ToString().Replace("\"", String.Empty));
                        returnObject.Add(value);

                        if (Object.DataType != null)
                        {
                            JProperty localType = new JProperty(Type, Object.DataType.ToString().Replace("\"", String.Empty));
                            returnObject.Add(localType);
                        }
                    }
                    else
                    {
                        if (Object.DataType != null)
                        {
                            object objectValue = new object();
                            if (Object.DataType.ToString()==Xsd.Boolean.ToString())
                            {
                                objectValue = Convert.ToBoolean(Object.Literal.ToString());
                                JProperty value = new JProperty(Value, objectValue);
                                returnObject.Add(value);                                
                            }
                            else if (Object.DataType.ToString() == Xsd.Integer.ToString())
                            {
                                objectValue = Convert.ToInt64(Object.Literal.ToString());
                                JProperty value = new JProperty(Value, objectValue);
                                returnObject.Add(value);
                            }
                            else if (Object.DataType.ToString() == Xsd.Double.ToString())
                            {
                                double doubleVal;
                                double.TryParse(Object.Literal.ToString(), System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture, out doubleVal).ToString();
                                objectValue = doubleVal;
                                JProperty value = new JProperty(Value, objectValue);
                                returnObject.Add(value);
                            }
                            else
                            {
                                objectValue = Object.ToString().Replace("\"", String.Empty);
                                JProperty value = new JProperty(Value, objectValue);
                                returnObject.Add(value);
                                JProperty localType = new JProperty(Type, Object.DataType.ToString());
                                returnObject.Add(localType);
                            }
                        }
                    }

                    if (Object.Language != null)
                    {
                        JProperty language = new JProperty(Language, Object.Language.ToString().Replace("\"", String.Empty));
                        returnObject.Add(language);
                    }
                } 

            return returnObject;
        }

        private string GetJsonPropertyForPredicate(JObject context, Node node)
        {
            return node.Uri.ToString();
        }

        private JArray WrapChildrenInArray(IEnumerable<JToken> children)
        {
            return new JArray(children.ToArray());
        }
    }
}
