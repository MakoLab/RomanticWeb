using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomanticWeb.Model;

namespace RomanticWeb.JsonLd
{
    public class JsonLdSerializer:IJsonLdProcessor
    {
        internal const string Id = "@id";
        internal const string Type = "@type";
        internal const string Language = "@language";
        internal const string Value = "@value";
        internal const string Context = "@context";
        internal const string Graph = "@graph";

        public string FromRdf(IEnumerable<EntityQuad> dataset,bool userRdfType=false,bool useNativeTypes=false)
        {
            return GetJsonStructure(dataset).ToString();
        }

        public IEnumerable<EntityQuad> ToRdf(string json)
        {
            throw new NotImplementedException();
        }

        public string Compact(string json,string jsonLdContext)
        {
            throw new NotImplementedException();
        }

        public string Flatten(string json,string jsonLdContext)
        {
            throw new NotImplementedException();
        }

        public string Expand(string json)
        {
            throw new NotImplementedException();
        }

        private JObject GetJsonStructure(IEnumerable<EntityQuad> quads)
        {
            var root = new JObject();
            var context = new JObject();

            var blankNodes = new List<Node>(quads.Where(quad => quad.Subject.IsBlank).Join(quads, quad => quad.Subject, quad => quad.Object, (outer, inner) => inner.Object).Distinct());
            var topLevelSubjects = quads.Where(quad => (!blankNodes.Contains(quad.Subject))).Select(x => x.Subject).Distinct();

            var serialized = topLevelSubjects.Select(subject => SerializeEntity(subject, context, quads)).ToList();

            root[Context] = context;
            root[Graph] = new JArray(serialized);

            return root;
        }

        private JObject SerializeEntity(Node subject, JObject context, IEnumerable<EntityQuad> quads)
        {
            var groups = from quad in quads
                         where quad.Subject == subject
                         group quad.Object by quad.Predicate into g
                         select new
                         {
                             Predicate = g.Key,
                             Objects = g
                         };

            var result = new JObject();
            foreach (var g in groups)
            {
                IList<JToken> children = new List<JToken>();
                string predicate = GetJsonPropertyForPredicate(context, g.Predicate);

                foreach (var obj in g.Objects)
                {
                    if (obj.IsLiteral)
                    {
                        children.Add(new JValue(obj.Literal));
                    }
                    else
                    {
                        children.Add(SerializeEntity(obj, context, quads));
                    }

                    if (children.Count == 1)
                    {
                        result[predicate] = children.Single();
                    }
                    else
                    {
                        JArray array = WrapChildrenInArray(children);
                        result[predicate] = array;
                    }
                }
            }

            return result;
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
