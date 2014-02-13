using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.JsonLd
{
    public class JsonLdProcessor : IJsonLdProcessor
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
        private JArray listInGraph = new JArray();
        private bool nativeTypes = false;
        private List<Node> nodesInList = new List<Node>();

        public string FromRdf(IEnumerable<EntityQuad> dataset, bool userRdfType = false, bool useNativeTypes = false)
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

        public string Flatten(string json, string jsonLdContext)
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
            IDictionary<JToken, JObject> subjectMap = new Dictionary<JToken, JObject>();
            var dataset = from triple in quads
                          group triple by triple.Graph into g
                          select g;

            foreach (var graph in dataset)
            {
                var lists = GetListsFromGraph(graph);

                var graphLocal = graph;
                var distinctSubjects = (from q in graph
                                        where q.Graph == graphLocal.Key && (!nodesInList.Contains(q.Subject))
                                        orderby q.Subject.IsBlank ? "_:" + q.Subject.BlankNode : q.Subject.ToString() // todo: remove ordering
                                        select q.Subject).Distinct();

                foreach (var subject in distinctSubjects)
                {
                    var entitySerialized = SerializeEntity(subject, graph, graph.Key, lists);

                    if (graph.Key != null)
                    {
                        var namedGraph = new JObject();
                        namedGraph[Id] = graph.Key.ToString();
                        namedGraph[Graph] = new JArray(entitySerialized);
                        entitySerialized = namedGraph;
                    }

                    if (subjectMap.ContainsKey(entitySerialized[Id]))
                    {
                        foreach (var property in entitySerialized.Properties())
                        {
                            subjectMap[entitySerialized[Id]][property.Name] = property.Value;
                        }
                    }
                    else
                    {
                        subjectMap[entitySerialized[Id]] = entitySerialized;
                    }
                }
            }

            return new JArray(subjectMap.Values);
        }

        private JObject SerializeEntity(Node subject, IEnumerable<EntityQuad> quads, Node graphName, JObject listsInGraph)
        {
            var groups = from quad in quads
                         where quad.Subject == subject && quad.Graph == graphName
                         group quad.Object by quad.Predicate into g
                         select new
                         {
                             Predicate = (g.Key == type ? rdfType : g.Key),
                             Objects = g
                         }
                             into selection
                             orderby selection.Predicate
                             select selection;

            JObject result = new JObject();
            int i = 0;

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
                                                     select ReturnProperties(o, listsInGraph))));
                if (i == 0)
                {
                    result.AddFirst(new JProperty(Id, subject.IsBlank ? "_:" + subject.BlankNode.ToString() : subject.ToString()));
                    i++;
                }

                result.Add(res);
            }

            return result;
        }

        private JObject GetListsFromGraph(IEnumerable<EntityQuad> quads)
        {
            var lists = quads.Where(q => (q.Subject.IsBlank && q.Predicate == rest && q.Object == nil));
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

            return locList;
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
            if (anotherPropertyInList > 2 || firstValues.Count() > 1)
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

        private JObject ReturnListProperties(Node @object)
        {
            JObject returnObject = new JObject();
            if (!@object.IsLiteral)
            {
                JProperty id = new JProperty(Id, @object.IsBlank ? "_:" + @object.BlankNode.ToString() : @object.ToString());
                returnObject.Add(id);
            }
            else
            {
                JProperty value = new JProperty(Value, @object.ToString().Replace("\"", String.Empty));
                returnObject.Add(value);

                if (@object.Language != null)
                {
                    JProperty language = new JProperty(Language, @object.Language.ToString().Replace("\"", String.Empty));
                    returnObject.Add(language);
                }

                if (@object.DataType != null)
                {
                    JProperty localType = new JProperty(Type, @object.DataType.ToString().Replace("\"", String.Empty));
                    returnObject.Add(localType);
                }
            }

            return returnObject;
        }

        private JObject ReturnProperties(Node @object, JObject lists)
        {
            JObject returnObject = new JObject();
            if (!@object.IsLiteral)
            {
                if (@object == nil)
                {
                    JProperty listProperty = new JProperty(List, new JArray());
                    returnObject.Add(listProperty);
                }
                else
                {
                    if (lists.Property(@object.ToString()) != null)
                    {
                        var localList = lists.Property(@object.ToString());
                        if ((object)localList != null)
                        {
                            returnObject.Add(new JProperty(List, localList.Value));
                        }
                    }
                    else
                    {
                        JProperty id = new JProperty(Id, @object.IsBlank ? "_:" + @object.BlankNode.ToString() : @object.ToString());
                        returnObject.Add(id);
                    }
                }
            }
            else
            {
                if (!nativeTypes)
                {
                    JProperty value = new JProperty(Value, @object.ToString().Replace("\"", String.Empty));
                    returnObject.Add(value);

                    if (@object.DataType != null)
                    {
                        JProperty localType = new JProperty(Type, @object.DataType.ToString().Replace("\"", String.Empty));
                        returnObject.Add(localType);
                    }
                }
                else
                {
                    if (@object.DataType != null)
                    {
                        object objectValue = new object();
                        if (@object.DataType.ToString() == Xsd.Boolean.ToString())
                        {
                            objectValue = Convert.ToBoolean(@object.Literal.ToString());
                            JProperty value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                        }
                        else if (@object.DataType.ToString() == Xsd.Integer.ToString())
                        {
                            objectValue = Convert.ToInt64(@object.Literal.ToString());
                            JProperty value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                        }
                        else if (@object.DataType.ToString() == Xsd.Double.ToString())
                        {
                            double doubleVal;
                            double.TryParse(@object.Literal.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out doubleVal).ToString();
                            objectValue = doubleVal;
                            JProperty value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                        }
                        else
                        {
                            objectValue = @object.ToString().Replace("\"", String.Empty);
                            JProperty value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                            JProperty localType = new JProperty(Type, @object.DataType.ToString());
                            returnObject.Add(localType);
                        }
                    }
                }

                if (@object.Language != null)
                {
                    JProperty language = new JProperty(Language, @object.Language.ToString().Replace("\"", String.Empty));
                    returnObject.Add(language);
                }
            }

            return returnObject;
        }
    }
}
