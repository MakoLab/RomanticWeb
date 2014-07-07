using System;
using Newtonsoft.Json.Linq;

namespace RomanticWeb.JsonLd
{
    internal class TermDefinition : ICloneable
    {
        public string Iri { get; set; }

        public bool IsReverse { get; set; }

        public string Type { get; set; }

        public string Language { get; set; }

        public string Container { get; set; }

        public JToken Original { get; set; }

        public JObject Object { get { return (Original is JObject) ? (JObject)Original : null; } }

        public JValue Value { get { return (Original is JValue ? (JValue)Original : null); } }

        public TermDefinition Clone()
        {
            return new TermDefinition()
            {
                Iri = Iri,
                IsReverse = IsReverse,
                Type = Type,
                Language = Language,
                Container = Container,
                Original = Original.DeepClone()
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}