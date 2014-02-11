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
        internal const string Type = "@type";
        internal const string Language = "@language";
        internal const string Value = "@value";
        internal const string Context = "@context";
        internal const string Graph = "@graph";
        private Node first = Node.ForUri(Rdf.first);
        private Node rest = Node.ForUri(Rdf.rest);
        private Node nil = Node.ForUri(Rdf.nil);
        private Node type = Node.ForUri(Rdf.type);
        private IEnumerable<Node> _distinctGrafs;
        private List<Node> listToInitiation = new List<Node>();
        private JArray listInGraph = new JArray();
        JObject resLists = new JObject();
        List<Node> nodesInList = new List<Node>();
        Node lastNodeInList = Node.ForUri(new Uri("http://example.com"));

        public string FromRdf(IEnumerable<EntityQuad> dataset,bool userRdfType=false,bool useNativeTypes=false)
        {
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
            ReturnLists(quads);
            var root = new JArray();
            var context = new JObject();
            //var blankNodes = new List<Node>(quads.Where(quad => quad.Subject.IsBlank)
            //                                                            .Join(quads, quad => quad.Subject, quad => quad.Object, (outer, inner) => inner.Object).Distinct());
            var topLevelSubjects = quads.Where(quad => (!nodesInList.Contains(quad.Subject))).Select(x => x.Subject).Distinct();
             _distinctGrafs = quads.Where(graph=>graph.Graph!=null).Select(x => x.Graph).Distinct();

             //var distinctSubjects = quads.Where(graph => (graph.Graph == null) && (!(graph.Predicate == first && graph.Object.IsBlank))
             //                                                                && (!(graph.Subject.IsBlank && (graph.Predicate == rest || graph.Predicate == first))))
             //                        .OrderBy(x => x.Subject.IsBlank ? "_:" + x.Subject.BlankNode.ToString() : x.Subject.ToString()).Select(x => x.Subject).Distinct();
             var distinctSubjects = quads.Where(graph => (graph.Graph == null && (!nodesInList.Contains(graph.Subject))))
                         .OrderBy(x => x.Subject.IsBlank ? "_:" + x.Subject.BlankNode.ToString() : x.Subject.ToString()).Select(x => x.Subject).Distinct();
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
                    var graphDistinctSubjects = quads.Where(graph => graph.Graph == dGraph && graph.Predicate != first && graph.Predicate != rest)
                                    .OrderBy(x => x.Subject.IsBlank ? "_:" + x.Subject.BlankNode.ToString() : x.Subject.ToString()).Select(x => x.Subject).Distinct();
                    var graphSerialized = graphDistinctSubjects.Select(subject => SerializeEntity(subject, context, quads, dGraph)).ToList();
                    JObject resultGraph = new JObject(new JProperty(Id,dGraph.ToString()),new JProperty(Graph, graphSerialized));
                    root.Add(resultGraph);
                }
            }
            
            return root;        
        }

        private JObject SerializeEntity(Node subject, JObject context, IEnumerable<EntityQuad> quads, Node graphName)
        {
            var groups = from quad in quads
                         where quad.Subject == subject && quad.Graph == graphName
                         group quad.Object by quad.Predicate into g
                         select new
                         {
                             Predicate =(g.Key == type ? Node.ForLiteral(Type) : g.Key),
                             Objects = g
                         } 
                         into selection
                         orderby selection.Predicate
                         select selection;

            JObject result = new JObject();
            int i=0;
                        
            foreach (var g in groups)
            {
                JProperty res = g.Predicate.ToString().Replace("\"", "") == Type ?
                            new JProperty(
                                new JProperty(Type,
                                    new JArray(
                                         from o in g.Objects
                                         select o.ToString())))
                :
                            new JProperty(
                                new JProperty(g.Predicate.ToString(),
                                    new JArray(
                                         from o in g.Objects
                                         select ReturnProperties(true,o, graphName, quads))));
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
                    _distinctGrafs=_distinctGrafs.Where(gr => gr.ToString() != subject.ToString());
                }
            }

        return result;
        }

        private void ReturnLists(IEnumerable<EntityQuad> quads)
        {
            var lists = quads.Where(q => (q.Subject.IsBlank && q.Predicate == rest && q.Object == nil));
            if (lists.Count() > 0)
            {
                
                foreach(var list in lists)
                {
                    listInGraph.Clear();
                    //JObject resList = new JObject();
                    PrepareLists(list, quads);
                    string indexer = listInGraph.First().ToString();
                    listInGraph.First.Remove();
                    resLists.Add(new JProperty(indexer,new JArray(listInGraph)));
                    listInGraph.Clear();
                }
            }
        }

        private  void PrepareLists(EntityQuad list, IEnumerable<EntityQuad> quads)
        {
             JObject result = new JObject();
             
             
             Node localSubject = list.Subject;
             //int anotherPropertyinList = quads.Where(q => q.Subject == localSubject && q.Predicate != first && q.Predicate != rest).Count();
             int anotherPropertyInList = quads.Where(q => q.Subject == localSubject && q.Predicate != type).Count();
             var firstValues = quads.Where(q => (q.Subject == localSubject && q.Predicate == first)).Select(q => q.Object);
             Node firstValue = firstValues.First();///////////////////////////////////////////////////
             Node restValue = list.Object;
                     
             //if (firstValue.IsLiteral)
             //{
                 listInGraph.AddFirst(ReturnListProperties(firstValue));
                 nodesInList.Add(localSubject);
                 lastNodeInList = localSubject;
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
                     if (firstQuad.Count() == 1)//restValue.IsBlank && firstValue.IsBlank
                     {
                         listInGraph.First.Remove();
                         nodesInList = nodesInList.Where(n => n != localSubject).ToList(); ////////////////////localSubject
                         listInGraph.AddFirst((restValue.IsBlank && firstValue.IsBlank) ? localSubject.ToString() : restValue.ToString());
                     }
                     else
                     {
                         listInGraph.AddFirst(localSubject.ToString());
                     }
                 }
                 
             //}
             //else
             //{
             //    localList.AddFirst(localList.ToString());
             //}
           //  return result;
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
                JProperty value = new JProperty(Value, Object.ToString().Replace("\"", ""));
                returnObject.Add(value);

                if (Object.Language != null)
                {
                    JProperty language = new JProperty(Language, Object.Language.ToString().Replace("\"", ""));
                    returnObject.Add(language);
                }

                if (Object.DataType != null)
                {
                    JProperty type = new JProperty(Type, Object.DataType.ToString().Replace("\"", ""));
                    returnObject.Add(type);
                }
            } 
            return returnObject;
        }

        private JObject ReturnProperties(bool NestedList,Node Object, Node Graph, IEnumerable<EntityQuad> quads)
        {
            JObject returnObject = new JObject();
            if (!Object.IsLiteral)
                {
                    if (Object == nil)
                    {
                        JProperty listProperty = new JProperty("@list", new JArray());
                        returnObject.Add(listProperty);
                    }
                    else
                    {
                        var localList = resLists.Property(Object.ToString());
                        if ((object)localList != null)
                        {
                            returnObject.Add(new JProperty("@list",localList.Value));
                        }
                        //var localQuads = quads.Where(q => q.Subject == Object && Object.IsBlank && q.Graph == Graph && (q.Predicate == first || q.Predicate == rest));
                        //   if (localQuads.Count() > 0 && NestedList)
                        //   {
                        //       JProperty listProperty;
                        //       List<Node> list = new List<Node>(GetList(Graph, quads, localQuads));
                        //       listProperty = new JProperty("@list", new JArray(list.Select(o => ReturnProperties(false, o, Graph, quads))));
                        //       returnObject.Add(listProperty);
                        //   }
                        else
                        {
                            JProperty id = new JProperty(Id, Object.IsBlank ? "_:" + Object.BlankNode.ToString() : Object.ToString());
                            returnObject.Add(id);
                        }
                    }
                }
                else
                {
                    JProperty value = new JProperty(Value,Object.ToString().Replace("\"", ""));
                    returnObject.Add(value);

                    if (Object.Language != null)
                    {
                        JProperty language = new JProperty(Language, Object.Language.ToString().Replace("\"", ""));
                        returnObject.Add(language);
                    }

                    if (Object.DataType != null)
                    {
                        JProperty type = new JProperty(Type, Object.DataType.ToString().Replace("\"", ""));
                        returnObject.Add(type);
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
