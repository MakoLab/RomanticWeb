using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.JsonLd
{
    public partial class JsonLdProcessor:IJsonLdProcessor
    {
        // TODO: Confirm that toRdf-0009 test has a typo in the input file ('foo' instead of 'foo:').
        // TODO: Confirm that Object to RDF conversion algorithm's step 8 is unnsecessary.
        // TODO: Confirm that Object to RDF conversion algorithm misses an xsd:dateTime, xsd:date and xsd:time datatypes related steps.
        // TODO: Confirm that Node map generation algorithm should have a 'reference' instead of 'element' in the step 6.6.3.
        // TODO: Confirm that Unit test #044 should have 'http://example.org/set2' instead of second 'http://example.org/set1'.
        public IEnumerable<EntityQuad> ToRdf(string json,JsonLdOptions options,bool produceGeneralizedRdf=false)
        {
            json=Expand(json,options);
            JObject nodeMap=new JObject();
            int counter=0;
            IDictionary<string,string> identifierMap=new Dictionary<string,string>();
            int counterGraphs=0;
            IDictionary<string,string> graphMap=new Dictionary<string,string>();
            GenerateNodeMap((JToken)JsonConvert.DeserializeObject(json),nodeMap,identifierMap,ref counter);
            List<EntityQuad> dataset=new List<EntityQuad>();
            foreach (JProperty _graph in nodeMap.Properties().OrderBy(graph => graph.Name==Default?1:0).ThenBy(graph => graph.Name))
            {
                string graphName=_graph.Name;
                JObject graph=(JObject)_graph.Value;
                if ((graphName!=Default)&&(IsRelative(graphName)))
                {
                    continue;
                }

                List<Triple> triples=new List<Triple>();
                foreach (JProperty _subject in graph.Properties().OrderBy(subject => subject.Name))
                {
                    string subject=_subject.Name;
                    JObject node=(JObject)_subject.Value;
                    if ((!Regex.IsMatch(subject,"[a-zA-Z0-9_]+://.+"))&&(!subject.StartsWith("_:")))
                    {
                        continue;
                    }

                    foreach (JProperty _property in node.Properties().OrderBy(property => property.Name))
                    {
                        string property=_property.Name;
                        if (property==Type)
                        {
                            foreach (JValue type in (JArray)_property.Value)
                            {
                                triples.Add(new Triple(CreateNode(subject),Node.a,CreateNode(type.ValueAs<string>())));
                            }
                        }
                        else if (IsKeyWord(property))
                        {
                            continue;
                        }
                        else if ((property.StartsWith("_:"))&&(!produceGeneralizedRdf))
                        {
                            continue;
                        }
                        else if (IsRelative(property))
                        {
                            continue;
                        }
                        else
                        {
                            JArray values=(JArray)_property.Value;
                            foreach (JToken item in values)
                            {
                                if ((item is JObject)&&(((JObject)item).IsPropertySet(List)))
                                {
                                    IList<Triple> listTriples=new List<Triple>();
                                    Node listHead=ConvertList((JArray)((JObject)item)[List],listTriples,identifierMap,ref counter,graphName,subject);
                                    triples.Add(new Triple(CreateNode(subject),Node.ForUri(new Uri(property)),listHead));
                                    triples.AddRange(listTriples);
                                }
                                else
                                {
                                    Node result=ConvertObject(item,ref counter,identifierMap);
                                    if (result!=null)
                                    {
                                        triples.Add(new Triple(CreateNode(subject,graphName),CreateNode(property),result));
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Triple triple in triples)
                {
                    if (graphName==Default)
                    {
                        dataset.Add(new EntityQuad(triple.Subject.ToEntityId(),triple.Subject,triple.Predicate,triple.Object));
                    }
                    else
                    {
                        dataset.Add(new EntityQuad(
                            new EntityId(triple.Subject.Uri),
                            triple.Subject,
                            triple.Predicate,
                            triple.Object,
                            CreateNode((IsBlankIri(graphName)?CreateBlankNodeId(graphName,graphMap,ref counterGraphs):graphName),graphName)));
                    }
                }
            }

            return dataset.Distinct();
        }

        private void GenerateNodeMap(JToken element,JObject nodeMap,IDictionary<string,string> identifierMap,ref int counter,string activeGraph=Default,JToken activeSubject=null,string activeProperty=null,JObject list=null)
        {
            if (element is JArray)
            {
                GenerateNodeMap((JArray)element,nodeMap,identifierMap,ref counter,activeGraph,activeSubject,activeProperty,list);
                return;
            }

            GenerateNodeMap((JObject)element,nodeMap,identifierMap,ref counter,activeGraph,activeSubject,activeProperty,list);
        }

        private void GenerateNodeMap(JArray element,JObject nodeMap,IDictionary<string,string> identifierMap,ref int counter,string activeGraph=Default,JToken activeSubject=null,string activeProperty=null,JObject list=null)
        {
            foreach (JToken item in (JArray)element)
            {
                GenerateNodeMap(item,nodeMap,identifierMap,ref counter,activeGraph,activeSubject,activeProperty,list);
            }
        }

        private void GenerateNodeMap(JObject element,JObject nodeMap,IDictionary<string,string> identifierMap,ref int counter,string activeGraph=Default,JToken activeSubject=null,string activeProperty=null,JObject list=null)
        {
            JObject graph=(JObject)nodeMap[activeGraph];
            if (graph==null)
            {
                nodeMap[activeGraph]=graph=new JObject();
            }

            JObject node=null;
            if (activeSubject!=null)
            {
                node=(JObject)graph[activeSubject.ToString()];
            }

            if (element.IsPropertySet(Type))
            {
                foreach (JValue item in (JArray)element[Type].AsArray())
                {
                    if (item.ValueAs<string>().StartsWith("_:"))
                    {
                        item.Value=CreateBlankNodeId(item.ValueAs<string>(),identifierMap,ref counter);
                    }
                }
            }

            if (element.IsPropertySet(Value))
            {
                if (list==null)
                {
                    if (!node.IsPropertySet(activeProperty))
                    {
                        node[activeProperty]=new JArray(element);
                    }
                    else if (!((JArray)node[activeProperty]).Any(item => element.Equals(item)))
                    {
                        ((JArray)node[activeProperty]).Add(element);
                    }
                }
                else
                {
                    ((JArray)list[List]).Add(element);
                }
            }
            else if (element.IsPropertySet(List))
            {
                JObject result=new JObject(new JProperty(List,new JArray()));
                GenerateNodeMap(element[List],nodeMap,identifierMap,ref counter,activeGraph,activeSubject,activeProperty,result);
                ((JArray)node[activeProperty]).Add(result);
            }
            else
            {
                string id;
                if (element.IsPropertySet(Id))
                {
                    id=((id=element.Property(Id).ValueAs<string>()).StartsWith("_:")?CreateBlankNodeId(id,identifierMap,ref counter):id);
                    element.Remove(Id);
                }
                else
                {
                    id=CreateBlankNodeId(null,identifierMap,ref counter);
                }

                if (!graph.IsPropertySet(id))
                {
                    graph[id]=new JObject(new JProperty(Id,id));
                }

                if (activeSubject is JObject)
                {
                    node=(JObject)graph[id];
                    if (!node.IsPropertySet(activeProperty))
                    {
                        node[activeProperty]=new JArray(activeSubject);
                    }
                    else if (!((JArray)node[activeProperty]).Any(item => activeSubject.Equals(item)))
                    {
                        ((JArray)node[activeProperty]).Add(activeSubject);
                    }
                }
                else if (activeProperty!=null)
                {
                    JObject reference=new JObject(new JProperty(Id,id));
                    if (list==null)
                    {
                        if (!node.IsPropertySet(activeProperty))
                        {
                            node[activeProperty]=new JArray(reference);
                        }
                        else if (!((JArray)node[activeProperty]).Any(item => reference.Equals(item)))
                        {
                            ((JArray)node[activeProperty]).Add(reference);
                        }
                    }
                    else
                    {
                        ((JArray)list[List]).Add(reference);
                    }
                }

                node=(JObject)graph[id];
                if (element.IsPropertySet(Type))
                {
                    foreach (JValue type in (JArray)element[Type])
                    {
                        if (!(node.IsPropertySet(Type)?(JArray)node[Type]:node[Type]=new JArray()).Contains(type))
                        {
                            ((JArray)node[Type]).Add(type);
                        }
                    }

                    element.Remove(Type);
                }

                if (element.IsPropertySet(Index))
                {
                    if ((node.IsPropertySet(Index))&&(!node[Index].Equals(element[Index])))
                    {
                        throw new InvalidOperationException("Conflicting indexes.");
                    }

                    node[Index]=element[Index];
                    element.Remove(Index);
                }

                if (element.IsPropertySet(Reverse))
                {
                    JObject referencedNode=new JObject(new JProperty(Id,id));
                    JObject reverseMap=(JObject)element[Reverse];
                    foreach (JProperty _property in reverseMap.Properties())
                    {
                        string property=_property.Name;
                        JArray values=(JArray)_property.Value;
                        foreach (JToken value in values)
                        {
                            GenerateNodeMap(value,nodeMap,identifierMap,ref counter,activeGraph,referencedNode,property);
                        }
                    }

                    element.Remove(Reverse);
                }

                if (element.IsPropertySet(Graph))
                {
                    GenerateNodeMap(element[Graph],nodeMap,identifierMap,ref counter,id);
                    element.Remove(Graph);
                }

                foreach (JProperty _property in element.Properties().OrderBy(property => property.Name))
                {
                    string property=_property.Name;
                    JToken value=_property.Value;
                    if (property.StartsWith("_:"))
                    {
                        property=CreateBlankNodeId(property,identifierMap,ref counter);
                    }

                    if (!node.IsPropertySet(property))
                    {
                        node[property]=new JArray();
                    }

                    GenerateNodeMap(value,nodeMap,identifierMap,ref counter,activeGraph,id,property);
                }
            }
        }

        private string CreateBlankNodeId(string identifier,IDictionary<string,string> identifierMap,ref int counter)
        {
            if ((identifier!=null)&&(identifierMap.ContainsKey(identifier)))
            {
                return identifierMap[identifier];
            }

            string result="_:b"+counter.ToString();
            counter++;
            if (identifier!=null)
            {
                identifierMap[identifier]=result;
            }

            return result;
        }

        private Node ConvertList(JArray list,IList<Triple> listTriples,IDictionary<string,string> identifierMap,ref int counter,string graphName,string parent)
        {
            if (list==null)
            {
                return Node.nil;
            }
            else
            {
                IList<string> bnodes=new List<string>();
                foreach (JObject entry in list)
                {
                    bnodes.Add(CreateBlankNodeId(null,identifierMap,ref counter));
                }

                listTriples.Clear();
                for (int index=0; index<list.Count; index++)
                {
                    string subject=bnodes[index];
                    JObject item=(JObject)list[index];
                    Node @object=ConvertObject(item,ref counter,identifierMap);
                    if (@object!=null)
                    {
                        listTriples.Add(new Triple(CreateNode(subject,graphName),Node.first,@object));
                        Node rest=(index+1<bnodes.Count?CreateNode(bnodes[index+1]):Node.nil);
                        listTriples.Add(new Triple(CreateNode(subject),Node.rest,rest));
                    }
                }

                return (bnodes.Count>0?CreateNode(bnodes[0]):Node.nil);
            }
        }

        private Node ConvertObject(JToken item,ref int counter,IDictionary<string,string> identifierMap)
        {
            JObject @object=item as JObject;
            if ((IsNodeObject(item))&&(!IsBlankIri(@object.Property(Id).ValueAs<string>()))&&(!Regex.IsMatch(@object.Property(Id).ValueAs<string>(),"[a-zA-Z0-9_]+://.+")))
            {
                return null;
            }

            if (IsNodeObject(item))
            {
                return CreateNode(@object.Property(Id).ValueAs<string>());
            }
            else
            {
                JValue value=(JValue)@object[Value];
                string datatype=(@object.IsPropertySet(Type)?@object.Property(Type).ValueAs<string>():null);
                if ((value.ValueIs<bool>())&&((value.ValueAs<bool>()==true)||(value.ValueAs<bool>()==false)))
                {
                    value=new JValue(value.Value.ToString().ToLower());
                    if (datatype==null)
                    {
                        datatype=RomanticWeb.Vocabularies.Xsd.Boolean.AbsoluteUri;
                    }
                }
                else if (((value.ValueIs<double>())&&(Math.Floor(value.ValueAs<double>())!=value.ValueAs<double>()))||((value.ValueIs<int>())&&(datatype==RomanticWeb.Vocabularies.Xsd.Double.AbsoluteUri)))
                {
                    value=new JValue(value.ValueAs<double>().ToString("0.0################################E0",CultureInfo.InvariantCulture));
                    if (datatype==null)
                    {
                        datatype=RomanticWeb.Vocabularies.Xsd.Double.AbsoluteUri;
                    }
                }
                else if ((value.ValueIs<int>())||((value.ValueIs<double>())&&(Math.Floor(value.ValueAs<double>())==value.ValueAs<double>())&&(datatype==RomanticWeb.Vocabularies.Xsd.Integer.AbsoluteUri)))
                {
                    value=new JValue(value.Value.ToString());
                    if (datatype==null)
                    {
                        datatype=RomanticWeb.Vocabularies.Xsd.Integer.AbsoluteUri;
                    }
                }
                else if (datatype==RomanticWeb.Vocabularies.Xsd.Date.AbsoluteUri)
                {
                    value=new JValue(value.ValueAs<DateTime>().ToString("yyyy\\-MM\\-dd"));
                    if (datatype==null)
                    {
                        datatype=RomanticWeb.Vocabularies.Xsd.DateTime.AbsoluteUri;
                    }
                }
                else if ((value.ValueIs<DateTime>())||(datatype==RomanticWeb.Vocabularies.Xsd.DateTime.AbsoluteUri))
                {
                    value=new JValue(value.ValueAs<DateTime>().ToString("yyyy\\-MM\\-dd"+((datatype==RomanticWeb.Vocabularies.Xsd.DateTime.AbsoluteUri)||(datatype!=RomanticWeb.Vocabularies.Xsd.Date.AbsoluteUri)?"\\THH\\:mm\\:ssZ":string.Empty)));
                    if (datatype==null)
                    {
                        datatype=RomanticWeb.Vocabularies.Xsd.DateTime.AbsoluteUri;
                    }
                }
                else if ((value.ValueIs<TimeSpan>())||(datatype==RomanticWeb.Vocabularies.Xsd.Time.AbsoluteUri))
                {
                    value=new JValue(new Duration(value.ValueAs<TimeSpan>()).ToString());
                    if (datatype==null)
                    {
                        datatype=RomanticWeb.Vocabularies.Xsd.DateTime.AbsoluteUri;
                    }
                }
                ////else if (datatype==null)
                ////{
                ////    datatype=(@object.IsPropertySet(Language)?"http://www.w3.org/2001/XMLSchema#langString":"http://www.w3.org/2001/XMLSchema#string");
                ////}

                if (@object.IsPropertySet(Language))
                {
                    return Node.ForLiteral((string)value.Value,@object.Property(Language).ValueAs<string>());
                }
                else if (datatype!=null)
                {
                    return Node.ForLiteral((string)value.Value,(IsBlankIri(datatype)?new Uri("blank://"+datatype.Substring(2)):new Uri(datatype)));
                }
                else
                {
                    return Node.ForLiteral((string)value.Value);
                }
            }
        }

        private bool IsNodeObject(JToken token)
        {
            JObject @object=token as JObject;
            return (@object!=null)&&(!@object.IsPropertySet(Value))&&(!@object.IsPropertySet(List))&&(!@object.IsPropertySet(Set));
        }

        private Node CreateNode(string iri,string graphName=null)
        {
            if (IsBlankIri(iri))
            {
                return Node.ForBlank(iri.Substring(2),null,null);
            }
            else
            {
                return Node.ForUri(new Uri(iri));
            }
        }

        private Uri CreateUri(string iri)
        {
            return new Uri(IsBlankIri(iri)?"blank://"+iri.Substring(2):iri);
        }

        private bool IsBlankIri(string iri)
        {
            return iri.StartsWith("_:");
        }

        private bool IsRelative(string iri)
        {
            return (iri.Length==0)||(((Char.IsLetter(iri[0]))||(Char.IsDigit(iri[0])))&&(!Regex.IsMatch(iri,"[a-zA-Z0-9_]+:")))||(iri[0]=='/');
        }
    }
}