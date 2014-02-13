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
        /// <summary>
        /// @id
        /// </summary>
        private const string Id = "@id";

        /// <summary>
        /// @language
        /// </summary>
        private const string Language = "@language";

        /// <summary>
        /// @value
        /// </summary>
        private const string Value = "@value";

        /// <summary>
        /// @context
        /// </summary>
        private const string Context = "@context";

        /// <summary>
        /// @graph
        /// </summary>
        private const string Graph = "@graph";

        /// <summary>
        /// @type
        /// </summary>
        private const string Type = "@type";

        /// <summary>
        /// @list
        /// </summary>
        private const string List = "@list";

        /// <summary>
        /// rdf:type
        /// </summary>
        private readonly Node _rdfType = Node.ForUri(Rdf.type);

        /// <summary>
        /// rdf:first
        /// </summary>
        private readonly Node _rdfFirst = Node.ForUri(Rdf.first);

        /// <summary>
        /// rdf:rest
        /// </summary>
        private readonly Node _rdfRest = Node.ForUri(Rdf.rest);

        /// <summary>
        /// rdf:nil
        /// </summary>
        private readonly Node _rdfNil = Node.ForUri(Rdf.nil);
        private JArray listInGraph = new JArray();
        private List<Node> nodesInList = new List<Node>();

        public string FromRdf(IEnumerable<EntityQuad> quads, bool userRdfType = false, bool useNativeTypes = false)
        {
            IDictionary<JToken, JObject> subjectMap = new Dictionary<JToken, JObject>();
            var dataset = from triple in quads
                          group triple by triple.Graph into g
                          select g;

            foreach (var graph in dataset)
            {
                var lists = GetListsFromGraph(graph);
                IGrouping<Node, EntityQuad> graph1 = graph;
                var distinctSubjects = (from q in graph
                                        where q.Graph == graph1.Key && (!nodesInList.Contains(q.Subject))
                                        orderby q.Subject.IsBlank ? "_:" + q.Subject.BlankNode : q.Subject.ToString() // todo: remove ordering
                                        select q.Subject).Distinct();

                foreach (var subject in distinctSubjects)
                {
                    Node subject1 = subject;
                    var entitySerialized = SerializeEntity(subject, graph.Where(n => n.Subject == subject1), graph.Key, useNativeTypes, userRdfType, lists);

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

            return new JArray(subjectMap.Values).ToString();
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

        public string Expand(string json, JsonLdOptions options)
        {
            return json;
        }

        private JObject SerializeEntity(Node subject, IEnumerable<EntityQuad> quads, Node graphName, bool nativeTypes, bool useRdfType, JObject listsInGraph)
        {
            var groups = from quad in quads
                         where quad.Subject == subject && quad.Graph == graphName
                         group quad.Object by quad.Predicate into g
                         select new
                         {
                             Predicate = (g.Key == _rdfType ? useRdfType ? _rdfType : Node.ForLiteral(Type) : g.Key),
                             Objects = g
                         }
                             into selection
                             orderby selection.Predicate
                             select selection;

            var result = new JObject();
            int i = 0;

            foreach (var objectGroup in groups)
            {
                JProperty res;
                if (objectGroup.Predicate == _rdfType || objectGroup.Predicate == Node.ForLiteral(Type))
                {
                    if (useRdfType)
                    {
                        res = new JProperty(new JProperty(_rdfType.ToString(), new JArray(from o in objectGroup.Objects select GetProperties(o, nativeTypes, listsInGraph))));
                    }
                    else
                    {
                        res = new JProperty(new JProperty(Type, new JArray(from o in objectGroup.Objects select o.ToString())));
                    }
                }
                else
                {
                    res = new JProperty(new JProperty(objectGroup.Predicate.Uri.ToString(), new JArray(from o in objectGroup.Objects select GetProperties(o, nativeTypes, listsInGraph))));
                }

                if (i == 0)
                {
                    result.AddFirst(new JProperty(Id, subject.IsBlank ? "_:" + subject.BlankNode : subject.ToString()));
                    i++;
                }

                result.Add(res);
            }

            return result;
        }

        private JObject GetListsFromGraph(IEnumerable<EntityQuad> quads)
        {
            var lists = quads.Where(q => (q.Subject.IsBlank && q.Predicate == _rdfRest && q.Object == _rdfNil));
            var locList = new JObject();
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
            int anotherPropertyInList = quads.Where(q => q.Subject == localSubject && q.Predicate != _rdfType).Count();
            var firstValues = quads.Where(q => (q.Subject == localSubject && q.Predicate == _rdfFirst)).Select(q => q.Object);
            Node firstValue = firstValues.First();
            Node restValue = list.Object;
            listInGraph.AddFirst(ReturnListProperties(firstValue));
            nodesInList.Add(localSubject);

            IEnumerable<EntityQuad> quad = quads.Where(q => (q.Subject.IsBlank && q.Predicate == _rdfRest && q.Object == localSubject));
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
                    IEnumerable<EntityQuad> firstQuad = quads.Where(q => (q.Subject.IsBlank && q.Predicate == _rdfFirst && q.Object == localSubject));
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
            var returnObject = new JObject();
            if (!@object.IsLiteral)
            {
                var id = new JProperty(Id, @object.IsBlank ? "_:" + @object.BlankNode : @object.ToString());
                returnObject.Add(id);
            }
            else
            {
                var value = new JProperty(Value, @object.ToString().Replace("\"", String.Empty));
                returnObject.Add(value);

                if (@object.Language != null)
                {
                    var language = new JProperty(Language, @object.Language.Replace("\"", String.Empty));
                    returnObject.Add(language);
                }

                if (@object.DataType != null)
                {
                    var localType = new JProperty(Type, @object.DataType.ToString().Replace("\"", String.Empty));
                    returnObject.Add(localType);
                }
            }

            return returnObject;
        }

        private JObject GetProperties(Node @object, bool nativeTypes, JObject lists)
        {
            var returnObject = new JObject();
            if (!@object.IsLiteral)
            {
                if (@object == _rdfNil)
                {
                    var listProperty = new JProperty(List, new JArray());
                    returnObject.Add(listProperty);
                }
                else
                {
                    if (lists.Property(@object.ToString()) != null)
                    {
                        var localList = lists.Property(@object.ToString());
                        if (localList != null)
                        {
                            returnObject.Add(new JProperty(List, localList.Value));
                        }
                    }
                    else
                    {
                        var id = new JProperty(Id, @object.IsBlank ? "_:" + @object.BlankNode : @object.ToString());
                        returnObject.Add(id);
                    }
                }
            }
            else
            {
                if (!nativeTypes)
                {
                    var value = new JProperty(Value, @object.ToString().Replace("\"", String.Empty));
                    returnObject.Add(value);

                    if (@object.DataType != null)
                    {
                        var localType = new JProperty(Type, @object.DataType.ToString().Replace("\"", String.Empty));
                        returnObject.Add(localType);
                    }
                }
                else
                {
                    if (@object.DataType != null)
                    {
                        object objectValue;
                        if (@object.DataType.ToString() == Xsd.Boolean.ToString())
                        {
                            objectValue = Convert.ToBoolean(@object.Literal);
                            var value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                        }
                        else if (@object.DataType.ToString() == Xsd.Integer.ToString())
                        {
                            objectValue = Convert.ToInt64(@object.Literal);
                            var value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                        }
                        else if (@object.DataType.ToString() == Xsd.Double.ToString())
                        {
                            double doubleVal;
                            double.TryParse(@object.Literal, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out doubleVal);
                            objectValue = doubleVal;
                            var value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                        }
                        else
                        {
                            objectValue = @object.ToString().Replace("\"", String.Empty);
                            var value = new JProperty(Value, objectValue);
                            returnObject.Add(value);
                            var localType = new JProperty(Type, @object.DataType.ToString());
                            returnObject.Add(localType);
                        }
                    }
                }

                if (@object.Language != null)
                {
                    var language = new JProperty(Language, @object.Language.Replace("\"", String.Empty));
                    returnObject.Add(language);
                }
            }

            return returnObject;
        }
    }
}
