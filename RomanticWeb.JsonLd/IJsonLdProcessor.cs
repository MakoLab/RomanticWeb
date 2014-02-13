using System.Collections.Generic;
using RomanticWeb.Model;

namespace RomanticWeb.JsonLd
{
    public interface IJsonLdProcessor
    {
        string FromRdf(IEnumerable<EntityQuad> dataset,bool userRdfType=false,bool useNativeTypes=false);

        IEnumerable<EntityQuad> ToRdf(string json);

        string Compact(string json,string jsonLdContext);

        string Flatten(string json,string jsonLdContext);

        string Expand(string json, JsonLdOptions options);
    }
}