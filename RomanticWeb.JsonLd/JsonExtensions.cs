using Newtonsoft.Json.Linq;

namespace RomanticWeb.JsonLd
{
    internal static class JsonExtensions
    {
        public static JObject Merge(this JObject current,JObject toMerge)
        {
            foreach (var property in toMerge.Properties())
            {
                var currentValue=current[property.Name];

                if (currentValue==null)
                {
                    current[property.Name]=property.Value;
                    continue;
                }

                if (property.Name=="@graph")
                {
                    foreach (var element in property.Value)
                    {
                        ((JArray)currentValue).Add(element);
                    }
                }
            }

            return current;
        }
    }
}