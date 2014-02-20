using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.JsonLd
{
    // TODO: Confirm that Unit test #004 should have 'http://example.org/set2' instead of second 'http://example.org/set1'.

    public partial class JsonLdProcessor:IJsonLdProcessor
    {
        public string Expand(string json,JsonLdOptions options)
        {
            JToken data=(JToken)JsonConvert.DeserializeObject(json);
            IList<string> remoteContexts=new List<string>();
            Context activeContext=new Context() { BaseIri=options.BaseUri.ToString(), DocumentUri=options.BaseUri };
            if (!System.String.IsNullOrEmpty(options.ExpandContext))
            {
                JToken expandedContext=(JToken)JsonConvert.DeserializeObject(options.ExpandContext);
                if (expandedContext!=null)
                {
                    if (!(expandedContext is JArray))
                    {
                        expandedContext=new JArray(expandedContext);
                    }

                    foreach (JToken item in (JArray)expandedContext)
                    {
                        if (item is JObject)
                        {
                            foreach (JProperty property in ((JObject)item).Properties())
                            {
                                if (property.Name==Context)
                                {
                                    activeContext.Merge(ProcessContext(activeContext,property.Value,remoteContexts));
                                }
                            }
                        }
                    }
                }
            }

            JToken result=Expand(null,data,activeContext,remoteContexts);
            if ((result is JObject)&&(((JObject)result).Property(Graph)!=null)&&(((JObject)result).Properties().Count()==1))
            {
                result=((JObject)result).Property(Graph).Value;
            }

            if (result==null)
            {
                result=new JArray();
            }

            if ((result!=null)&&(!(result is JArray)))
            {
                result=new JArray(result);
            }

            return JsonConvert.SerializeObject(result);
        }

        private JToken Expand(string activeProperty,JToken element,Context activeContext,IList<string> remoteContexts)
        {
            if (element==null)
            {
                return null;
            }

            if (element is JValue)
            {
                if (activeProperty==Graph)
                {
                    return null;
                }
                else
                {
                    return Expand(activeProperty,(JValue)element,activeContext);
                }
            }

            if (element is JArray)
            {
                return Expand(activeProperty,(JArray)element,activeContext,remoteContexts);
            }

            return Expand(activeProperty,(JObject)element,activeContext,remoteContexts);
        }

        private JToken Expand(string activeProperty,JValue value,Context activeContext)
        {
            if (value.Value==null)
            {
                return null;
            }

            JObject result;
            if ((activeContext.ContainsKey(activeProperty))&&(activeContext[activeProperty].Type==Id))
            {
                result=new JObject();
                result[Id]=ExpandIri(value.Value,activeContext,(JObject)null,true);
                return result;
            }

            if ((activeContext.ContainsKey(activeProperty))&&(activeContext[activeProperty].Type==Vocab))
            {
                result=new JObject();
                result[Id]=ExpandIri(value.Value,activeContext,(JObject)null,true,true);
                return result;
            }

            result=new JObject();
            result[Value]=value;
            if ((activeContext.ContainsKey(activeProperty))&&(activeContext[activeProperty].Type!=null))
            {
                result[Type]=new Uri(activeContext[activeProperty].Type);
            }
            else if (value.Type==JTokenType.String)
            {
                if ((activeContext.ContainsKey(activeProperty))&&(activeContext[activeProperty].Language!=null))
                {
                    if (activeContext[activeProperty].Language!="iv")
                    {
                        result[Language]=activeContext[activeProperty].Language;
                    }
                }
                else if (activeContext.Language!=null)
                {
                    result[Language]=activeContext.Language;
                }
            }

            return result;
        }

        private JToken Expand(string activeProperty,JArray array,Context activeContext,IList<string> remoteContexts)
        {
            JArray result=new JArray();
            foreach (JToken item in array)
            {
                JToken expandedItem=Expand(activeProperty,item,activeContext,remoteContexts);
                if (((activeProperty==List)||((activeProperty!=null)&&((activeContext.ContainsKey(activeProperty))&&(activeContext[activeProperty].Container==List))))&&
                    ((expandedItem is JArray)||((expandedItem is JObject)&&(((JObject)expandedItem).Property(List)!=null))))
                {
                    throw new InvalidOperationException("List of lists.");
                }

                if (expandedItem is JArray)
                {
                    foreach (JToken arrayItem in (JArray)expandedItem)
                    {
                        result.Add(arrayItem);
                    }
                }
                else if (expandedItem!=null)
                {
                    result.Add(expandedItem);
                }
            }

            return result;
        }

        private JToken Expand(string activeProperty,JObject @object,Context activeContext,IList<string> remoteContexts)
        {
            JObject result=new JObject();
            foreach (JProperty _property in @object.Properties().OrderBy(item => item.Name))
            {
                string key=_property.Name;
                JToken value=_property.Value;
                if (key==Context)
                {
                    activeContext=ProcessContext(activeContext,(JToken)value,remoteContexts);
                    continue;
                }

                string expandedProperty=ExpandIri(key,activeContext,(JObject)null,false,true,null);
                JToken expandedValue=null;
                if ((expandedProperty==null)||((expandedProperty.IndexOf(':')==-1)&&(!IsKeyWord(expandedProperty))))
                {
                    continue;
                }

                if (IsKeyWord(expandedProperty))
                {
                    bool @continue;
                    expandedValue=ExpandKeyword(result,_property,activeProperty,expandedProperty,activeContext,remoteContexts,out @continue);
                    if (@continue)
                    {
                        continue;
                    }
                }
                else if ((activeContext.ContainsKey(key))&&(activeContext[key].Container==Language)&&(value is JObject))
                {
                    expandedValue=new JArray();
                    foreach (JProperty property in ((JObject)value).Properties().OrderBy(member => member.Name))
                    {
                        string language=property.Name;
                        JToken languageValue=property.Value;
                        if (!(languageValue is JArray))
                        {
                            languageValue=new JArray(languageValue);
                        }

                        foreach (JValue item in (JArray)languageValue)
                        {
                            if (item.Type!=JTokenType.String)
                            {
                                throw new InvalidOperationException("Invalid language map value.");
                            }

                            JObject languageMap=new JObject();
                            ((JArray)expandedValue).Merge(languageMap);
                            languageMap[Value]=item;
                            languageMap[Language]=language.ToLower();
                        }
                    }
                }
                else if ((activeContext.ContainsKey(key))&&(activeContext[key].Container==Index)&&(value is JObject))
                {
                    expandedValue=new JArray();
                    foreach (JProperty property in ((JObject)value).Properties().OrderBy(member => member.Name))
                    {
                        string index=property.Name;
                        JToken indexValue=property.Value;
                        if (!(indexValue is JArray))
                        {
                            indexValue=new JArray(indexValue);
                        }

                        indexValue=Expand(key,indexValue,activeContext,remoteContexts);
                        foreach (JObject item in (JArray)indexValue)
                        {
                            if (item.Property(Index)==null)
                            {
                                item[Index]=index;
                            }

                            ((JArray)expandedValue).Merge(item);
                        }
                    }
                }
                else
                {
                    expandedValue=Expand(key,value,activeContext,remoteContexts);
                }

                if (expandedValue==null)
                {
                    continue;
                }

                if ((activeContext.ContainsKey(key))&&(activeContext[key].Container==List))
                {
                    if (expandedValue is JArray)
                    {
                        JObject newValue=new JObject();
                        newValue[List]=expandedValue;
                        expandedValue=newValue;
                    }

                    if (((expandedValue is JObject)&&(((JObject)expandedValue).Property(List)==null))||(!(expandedValue is JObject)))
                    {
                        JArray list=new JArray(expandedValue);
                        expandedValue=new JObject();
                        ((JObject)expandedValue)[List]=list;
                    }
                }

                // TODO: Confirm that JSON-LN expansion algorithm in point 7.10 has unwanted 'Otherwise' at the beginning.
                /*else*/ if ((activeContext.ContainsKey(key))&&(activeContext[key].IsReverse))
                {
                    if (result.Property(Reverse)==null)
                    {
                        result[Reverse]=new JObject();
                    }

                    JObject reverseMap=(JObject)result[Reverse];
                    if (!(expandedValue is JArray))
                    {
                        expandedValue=new JArray(expandedValue);
                    }

                    foreach (JToken item in (JArray)expandedValue)
                    {
                        if ((item is JObject)&&((((JObject)item).Property(Value)!=null)||(((JObject)item).Property(List)!=null)))
                        {
                            throw new InvalidOperationException("Invalid reverse property value.");
                        }

                        if (reverseMap.Property(expandedProperty)==null)
                        {
                            reverseMap[expandedProperty]=new JArray();
                        }

                        ((JArray)reverseMap[expandedProperty]).Merge(item);
                    }
                }
                else
                {
                    if (result.Property(expandedProperty)==null)
                    {
                        result[expandedProperty]=new JArray();
                    }

                    ((JArray)result[expandedProperty]).Merge(expandedValue);
                }
            }

            if (result.Property(Value)!=null)
            {
                if ((result.Properties().Any(property => (property.Name!=Value)&&(property.Name!=Language)&&(property.Name!=Type)&&(property.Name!=Index)))||
                    ((result.Property(Language)!=null)&&(result.Property(Type)!=null)))
                {
                    throw new InvalidOperationException("Invalid value object.");
                }

                if (result.Property(Value).ValueEquals(null))
                {
                    return result=null;
                }
                else if ((result[Value].Type!=JTokenType.String)&&(result.Property(Language)!=null))
                {
                    throw new InvalidOperationException("Invalid language-tagged value.");
                }
                else if ((result.Property(Type)!=null)&&(!result.Property(Type).ValueIs<Uri>()))
                {
                    throw new InvalidOperationException("Invalid typed value.");
                }
            }
            else if ((result.Property(Type)!=null)&&(!(result[Type] is JArray)))
            {
                result[Type]=new JArray(result[Type]);
            }

            // TODO: Confirm that JSON-LD expansion algorithm has unwanted point 10.1.
            else ////if ((result.Property(Set)!=null)||(result.Property(List)!=null))
            ////{
            ////    if (!result.Properties().Any(member => member.Name==Index))
            ////    {
            ////        throw new InvalidOperationException("Invalid set or list object.");
            ////    }

                if (result.Property(Set)!=null)
                {
                    if (result[Set] is JArray)
                    {
                        return result[Set];
                    }
                    else
                    {
                        result=(JObject)result[Set];
                    }
                }
            ////}

            if ((result.Property(Language)!=null)&&(result.Properties().Count()==1))
            {
                result=null;
            }

            if ((activeProperty==null)||(activeProperty==Graph))
            {
                if ((result.Properties().Count()==0)||((result.Property(Value)!=null)||(result.Property(List)!=null)))
                {
                    result=null;
                }
                else if ((result.Property(Id)!=null)&&(result.Properties().Count()==1))
                {
                    result=null;
                }
            }

            return result;
        }

        private JToken ExpandKeyword(JObject result,JProperty _property,string activeProperty,string expandedProperty,Context activeContext,IList<string> remoteContexts,out bool @continue)
        {
            @continue=false;
            JToken expandedValue=null;
            string key=_property.Name;
            JToken value=_property.Value;
            if (activeProperty==Reverse)
            {
                throw new InvalidOperationException("Invalid reverse property map.");
            }

            if (result.Property(expandedProperty)!=null)
            {
                throw new InvalidOperationException("Colliding keywords.");
            }

            if (expandedProperty==Id)
            {
                if (!_property.ValueIs<string>())
                {
                    throw new InvalidOperationException("Invalid @id value.");
                }

                expandedValue=ExpandIri(_property.ValueAs<string>(),activeContext,(JObject)null,true);
            }

            if (expandedProperty==Type)
            {
                if ((!_property.ValueIs<string>())&&((!(value is JArray))||(((JArray)value).Count(item => item.Type==JTokenType.String)!=((JArray)value).Count)))
                {
                    throw new InvalidOperationException("Invalid type value.");
                }

                if (_property.ValueIs<string>())
                {
                    expandedValue=ExpandIri(_property.ValueAs<string>(),activeContext,(JObject)null,true,true);
                }
                else
                {
                    expandedValue=new JArray();
                    foreach (JValue type in (JArray)_property.Value)
                    {
                        ((JArray)expandedValue).Add(new JValue(ExpandIri((string)type.Value,activeContext,(JObject)null,true,true)));
                    }
                }
            }

            if (expandedProperty==Graph)
            {
                expandedValue=Expand(Graph,value,activeContext,remoteContexts);
            }

            if (expandedProperty==Value)
            {
                if ((!(value is JValue))||(value==null))
                {
                    throw new InvalidOperationException("Invalid value object value.");
                }

                expandedValue=value;

                if ((expandedValue==null)||(((JValue)expandedValue).Value==null))
                {
                    result[Value]=null;
                    @continue=true;
                    return expandedValue;
                }
            }

            if (expandedProperty==Language)
            {
                if (!_property.ValueIs<string>())
                {
                    throw new InvalidOperationException("Invalid language-tagged string.");
                }

                expandedValue=_property.ValueAs<string>().ToLower();
            }

            if (expandedProperty==Index)
            {
                if (!_property.ValueIs<string>())
                {
                    throw new InvalidOperationException("Invalid @index value.");
                }

                expandedValue=_property.ValueAs<string>();
            }

            if (expandedProperty==List)
            {
                if ((activeProperty==null)||(activeProperty==Graph))
                {
                    @continue=true;
                    return expandedValue;
                }

                expandedValue=Expand(activeProperty,value,activeContext,remoteContexts);
                if ((expandedValue is JObject)&&(((JObject)expandedValue).Property(List)!=null))
                {
                    throw new InvalidOperationException("List of lists.");
                }

                if (!(expandedValue is JArray))
                {
                    expandedValue=new JArray(expandedValue);
                }
            }

            if (expandedProperty==Set)
            {
                expandedValue=Expand(activeProperty,value,activeContext,remoteContexts);
            }

            if (expandedProperty==Reverse)
            {
                if (!(value is JObject))
                {
                    throw new InvalidOperationException("Invalid @reverse value.");
                }

                expandedValue=Expand(Reverse,value,activeContext,remoteContexts);
                if (expandedValue is JObject)
                {
                    if (((JObject)expandedValue).Property(Reverse)!=null)
                    {
                        foreach (JProperty property in ((JObject)((JObject)expandedValue).Property(Reverse).Value).Properties())
                        {
                            JToken item=property.Value;
                            if (result.Property(property.Name)==null)
                            {
                                result[property.Name]=new JArray();
                            }

                            ((JArray)result[property.Name]).Merge(item);
                        }
                    }

                    if (((JObject)expandedValue).Properties().Count(member => member.Name!=Reverse)>0)
                    {
                        JObject reverseMap;
                        if (result.Property(Reverse)==null)
                        {
                            result[Reverse]=reverseMap=new JObject();
                        }
                        else
                        {
                            reverseMap=(JObject)result[Reverse];
                        }

                        foreach (JProperty property in ((JObject)expandedValue).Properties())
                        {
                            if (property.Name!=Reverse)
                            {
                                JArray items=(JArray)property.Value;
                                foreach (JToken item in items)
                                {
                                    if ((item is JObject)&&((((JObject)item).Property(Value)!=null)||(((JObject)item).Property(List)!=null)))
                                    {
                                        throw new InvalidOperationException("Invalid reverse property value.");
                                    }

                                    if (reverseMap.Property(property.Name)==null)
                                    {
                                        reverseMap[property.Name]=new JArray();
                                    }

                                    ((JArray)reverseMap[property.Name]).Add(item);
                                }
                            }
                        }
                    }
                }

                @continue=true;
                return expandedValue;
            }

            if (expandedValue!=null)
            {
                result[expandedProperty]=expandedValue;
            }

            @continue=true;
            return expandedValue;
        }

        private string ExpandIri(object value,Context context,Context localContext=null,bool documentRelative=false,bool vocab=false,IDictionary<string,object> defined=null)
        {
            JObject newLocalContext=null;
            if (localContext!=null)
            {
                foreach (KeyValuePair<string,TermDefinition> termDefinition in localContext)
                {
                    newLocalContext[termDefinition.Key]=termDefinition.Value.Original;
                }
            }

            return ExpandIri(value,context,newLocalContext,documentRelative,vocab,defined);
        }

        private string ExpandIri(object value,Context context,JObject localContext=null,bool documentRelative=false,bool vocab=false,IDictionary<string,object> defined=null)
        {
            if (value==null) { return null; }
            string valueString=(value as string);
            if (valueString==null)
            {
                throw new InvalidOperationException(System.String.Format("Cannot expand iri of type '{0}'.",value.GetType()));
            }

            if (IsKeyWord(valueString)) { return valueString; }
            if ((localContext!=null)&&(localContext.Property(valueString)!=null))
            {
                if ((!defined.ContainsKey(valueString))||(defined[valueString] is bool)&&(!(bool)defined[valueString]))
                {
                    CreateTermDefinition(valueString,context,localContext,defined);
                }
            }

            if ((vocab)&&(context.ContainsKey(valueString)))
            {
                return (context[valueString]!=null?context[valueString].Iri:null);
            }

            if (Regex.IsMatch(valueString,"[a-zA-Z0-9_]+:.+"))
            {
                string prefix=valueString.Substring(0,valueString.IndexOf(':'));
                string suffix=valueString.Substring(prefix.Length+1);
                if ((prefix=="_")||(suffix.StartsWith("//")))
                {
                    return valueString;
                }

                if ((localContext!=null)&&((!defined.ContainsKey(prefix))||((defined[prefix] is bool)&&(!(bool)defined[prefix]))))
                {
                    CreateTermDefinition(prefix,context,localContext,defined);
                }

                if (context.ContainsKey(prefix))
                {
                    return context[prefix].Iri.ToString()+suffix;////MakeAbsoluteUri(context[prefix].Iri.ToString(),suffix);
                }

                return valueString;
            }

            if ((vocab)&&(context.Vocabulary!=null))
            {
                return MakeAbsoluteUri(context.Vocabulary,valueString);
            }

            if (documentRelative)
            {
                return MakeAbsoluteUri(context.BaseIri,valueString);
            }

            return valueString;
        }

        private void CreateTermDefinition(string term,Context context,Context localContext,IDictionary<string,object> defined)
        {
            JObject newLocalContext=new JObject();
            foreach (KeyValuePair<string,TermDefinition> termDefinition in localContext)
            {
                newLocalContext[termDefinition.Key]=termDefinition.Value.Original;
            }

            CreateTermDefinition(term,context,newLocalContext,defined);
        }

        private void CreateTermDefinition(string term,Context context,JObject localContext,IDictionary<string,object> defined)
        {
            if (defined.ContainsKey(term))
            {
                if ((defined[term] is bool)&&(!(bool)defined[term]))
                {
                    throw new InvalidOperationException("Cyclic IRI mapping detected.");
                }

                return;
            }

            if (IsKeyWord(term))
            {
                throw new InvalidOperationException("Keyword redefinition detected.");
            }

            defined[term]=false;
            context.Remove(term);
            JToken value=((localContext!=null)&&(localContext.Property(term)!=null)?localContext[term].DeepClone():null);
            if ((value==null)||((value is JValue)&&(((JValue)value).Value==null))||((value is JObject)&&(((JObject)value).Property(Id)!=null)&&(((JObject)value).Property(Id).ValueEquals(null))))
            {
                context[term]=null;
                return;
            }
            else if ((value is JValue)&&(((JValue)value).Type==JTokenType.String))
            {
                JObject newObject=new JObject();
                newObject[Id]=value;
                value=newObject;
            }
            else if (!(value is JObject))
            {
                throw new InvalidOperationException("Invalid term definition.");
            }

            TermDefinition definition=new TermDefinition() { Original=value };
            if (((JObject)value).Property(Type)!=null)
            {
                if (!((JObject)value).Property(Type).ValueIs<string>())
                {
                    throw new InvalidOperationException("Invalid type mapping.");
                }

                string result=((JObject)value).Property(Type).ValueAs<string>();
                result=ExpandIri(result,context,localContext,false,true,defined);
                if ((result!=Id)&&(result!=Vocab)&&(!Regex.IsMatch(result,"[a-zA-Z0-9_]+:.+")))
                {
                    throw new InvalidOperationException("Invalid type mapping.");
                }

                definition.Type=result;
            }
            if (((JObject)value).Property(Reverse)!=null)
            {
                JProperty reverse=((JObject)value).Property(Reverse);
                if (((JObject)value).Property(Id)!=null)
                {
                    throw new InvalidOperationException("Invalid reverse property.");
                }

                if (!reverse.ValueIs<string>())
                {
                    throw new InvalidOperationException("Invalid IRI mapping.");
                }

                definition.Iri=ExpandIri(reverse.ValueAs<string>(),context,localContext,false,true,defined);

                if (((JObject)value).Property(Container)!=null)
                {
                    definition.Container=(string)((JObject)value)[Container];
                }

                definition.IsReverse=true;
                context[term]=definition;
                defined[term]=true;
                return;
            }

            definition.IsReverse=false;
            if ((value is JObject)&&(((JObject)value).Property(Id)!=null)&&(!((JObject)value).Property(Id).ValueEquals(term)))
            {
                if ((!(((JObject)value).Property(Id).Value is JValue))||(((JValue)((JObject)value).Property(Id).Value).Type!=JTokenType.String))
                {
                    throw new InvalidOperationException("Invalid IRI mapping.");
                }

                string iri=ExpandIri((string)((JValue)((JObject)value).Property(Id).Value).Value,context,localContext,false,true,defined).ToString();
                if ((!IsKeyWord((string)iri))&&(!Regex.IsMatch(iri,"[a-zA-Z0-9_]+:.+")))
                {
                    throw new InvalidOperationException("Invalid IRI mapping.");
                }

                if ((string)iri==Context)
                {
                    throw new InvalidOperationException("Invalid keyword alias.");
                }

                definition.Iri=iri;
            }
            else if (term.IndexOf(':')!=-1)
            {
                string prefix=null;
                if (Regex.IsMatch(term,"[a-zA-Z0-9_]+:[a-zA-Z0-9_]+"))
                {
                    prefix=term.Substring(0,term.IndexOf(':'));
                    if (localContext.Property(prefix)!=null)
                    {
                        CreateTermDefinition(prefix,context,localContext,defined);
                    }
                }

                if ((prefix!=null)&&(context.ContainsKey(prefix)))
                {
                    definition.Iri=MakeAbsoluteUri(context[prefix].Iri,term.Substring(prefix.Length+1));
                }
                else
                {
                    definition.Iri=term;
                }
            }
            else if (context.Vocabulary!=null)
            {
                definition.Iri=MakeAbsoluteUri(context.Vocabulary,term);
            }
            else
            {
                throw new InvalidOperationException("Invalid IRI mapping.");
            }

            if (((JObject)value).Property(Container)!=null)
            {
                if ((!((JObject)value).Property(Container).ValueEquals(List))&&(!((JObject)value).Property(Container).ValueEquals(Set))&&
                    (!((JObject)value).Property(Container).ValueEquals(Index))&&(!((JObject)value).Property(Container).ValueEquals(Language)))
                {
                    throw new InvalidOperationException("Invalid container mapping.");
                }

                definition.Container=(string)((JValue)((JObject)value).Property(Container).Value).Value;
            }

            if ((((JObject)value).Property(Language)!=null)&&((((JObject)value).Property(Type)==null)||(((JObject)value).Property(Type).ValueEquals(null))))
            {
                if ((((JObject)value).Property(Language).Value is JValue)&&((!((JObject)value).Property(Language).ValueEquals(null))&&(!((JObject)value).Property(Language).ValueIs<string>())))
                {
                    throw new InvalidOperationException("Invalid lanuage mapping.");
                }

                definition.Language=(!((JObject)value).Property(Language).ValueEquals(null)?((JObject)value).Property(Language).ValueAs<string>().ToLower():"iv");
            }

            context[term]=definition;
            defined[term]=true;
        }

        private Context ProcessContext(Context activeContext,JToken localContext,IList<string> remoteContexts)
        {
            Context result=activeContext.Clone();
            JArray localContextArray=localContext as JArray;
            if (localContextArray==null)
            {
                localContextArray=new JArray(localContext);
            }

            foreach (JToken context in localContextArray)
            {
                if ((context==null)||((context is JValue)&&(((JValue)context).Value==null)))
                {
                    result=new Context();
                    result.BaseIri=(activeContext.DocumentUri!=null?activeContext.DocumentUri.ToString():null);
                    continue;
                }

                if ((context is JValue)&&(((JValue)context).Type==JTokenType.String))
                {
                    string contextIri=MakeAbsoluteUri(activeContext.BaseIri,(string)((JValue)context).Value);
                    if (remoteContexts.Contains(contextIri))
                    {
                        throw new InvalidOperationException("Recursive context inclusion.");
                    }

                    remoteContexts.Add(contextIri);
                    HttpWebRequest request=(HttpWebRequest)HttpWebRequest.Create(contextIri);
                    request.CookieContainer=new CookieContainer();
                    HttpWebResponse response=(HttpWebResponse)request.GetResponse();
                    JToken remoteContext=(JToken)JsonConvert.DeserializeObject(new StreamReader(response.GetResponseStream()).ReadToEnd());
                    response.Close();
                    if (!(remoteContext is JObject))
                    {
                        throw new InvalidOperationException("Invalid remote context");
                    }

                    JObject remoteContextObject=(JObject)remoteContext;
                    if (remoteContextObject.Property(Context)==null)
                    {
                        throw new InvalidOperationException("Invalid remote context");
                    }

                    result=ProcessContext(result,context,remoteContexts);
                    continue;
                }

                if (!(context is JObject))
                {
                    throw new InvalidOperationException("Invalid local context");
                }

                JObject contextObject=(JObject)context;
                if ((contextObject.Property(Base)!=null)&&(remoteContexts.Count==0))
                {
                    string value=contextObject.Property(Base).ValueAs<string>();
                    if (value==null)
                    {
                        result.BaseIri=null;
                    }
                    else if (Regex.IsMatch(value,"[a-zA-Z0-9_]+://.+"))
                    {
                        result.BaseIri=value;
                    }
                    else if ((!Regex.IsMatch(value,"[a-zA-Z0-9_]+:.+"))&&(result.BaseIri!=null))
                    {
                        result.BaseIri=MakeAbsoluteUri(result.BaseIri,value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid base IRI.");
                    }
                }

                if (contextObject.Property(Vocab)!=null)
                {
                    string value=contextObject.Property(Vocab).ValueAs<string>();
                    if (value==null)
                    {
                        result.Vocabulary=null;
                    }
                    else if ((Regex.IsMatch(value,"[a-zA-Z0-9_]+://"))||(Regex.IsMatch(value,"_:")))
                    {
                        result.Vocabulary=value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid vocab mapping.");
                    }
                }

                if (contextObject.Property(Language)!=null)
                {
                    if (contextObject.Property(Language).ValueEquals(null))
                    {
                        result.Language=null;
                    }
                    else if (!contextObject.Property(Language).ValueIs<string>())
                    {
                        throw new InvalidOperationException("Invalid default language.");
                    }
                    else
                    {
                        result.Language=contextObject.Property(Language).ValueAs<string>().ToLower();
                    }
                }

                IDictionary<string,object> defined=new Dictionary<string,object>();
                foreach (JProperty property in contextObject.Properties())
                {
                    if ((property.Name!=Base)&&(property.Name!=Vocab)&&(property.Name!=Language))
                    {
                        CreateTermDefinition(property.Name,result,contextObject,defined);
                    }
                }
            }

            return result;
        }

        private bool IsKeyWord(string token)
        {
            return ((token==Id)||(token==Language)||(token==Value)||(token==Context)||(token==Graph)||(token==Type)||(token==List)||(token==Vocab)||(token==Reverse)||(token==Container)||(token==Base)||(token==Set)||(token==Index));
        }

        private string MakeAbsoluteUri(string baseUriString,string relativeUriString)
        {
            if (baseUriString==null)
            {
                return relativeUriString;
            }

            if (baseUriString.StartsWith("_:"))
            {
                return "_:"+baseUriString.Substring(2)+relativeUriString;
            }

            Uri baseUri=new Uri(baseUriString,UriKind.Absolute);
            if ((baseUri.Fragment.Length>1)||(relativeUriString.StartsWith("#"))||(relativeUriString.StartsWith("?"))||(relativeUriString.StartsWith("/")))
            {
                return new Uri(baseUri,relativeUriString).ToString();
            }
            else
            {
                if (baseUri.Fragment.Length==0)
                {
                    UriBuilder uriBuilder=new UriBuilder(baseUri);
                    if (!baseUri.Segments.Last().EndsWith("/"))
                    {
                        uriBuilder.Path=System.String.Join("",baseUri.Segments,0,baseUri.Segments.Length-1);
                        baseUri=uriBuilder.Uri;
                    }
                }
                return new Uri(baseUri.ToString()+relativeUriString,UriKind.Absolute).ToString();
            }
        }
    }
}