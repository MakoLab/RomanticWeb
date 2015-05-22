using System.Collections.Generic;
using System.Globalization;
using RomanticWeb.Model;

namespace RomanticWeb.Entities
{
    internal static class EntityProxyHelpers
    {
        internal static IEnumerable<Node> WhereMatchesContextRequirements(this IEnumerable<Node> nodes, IEntityContext context)
        {
            var invariantCandidates = new List<Node>();
            var objects = new List<Node>();
            bool ignoreInvariantNodes = false;
            foreach (var node in nodes)
            {
                if (!node.IsLiteral)
                {
                    objects.Add(node);
                }
                else
                {
                    if (context.CurrentCulture.Equals(CultureInfo.InvariantCulture))
                    {
                        if (node.Language == null)
                        {
                            objects.Add(node);
                        }
                    }
                    else
                    {
                        if (node.Language == null)
                        {
                            invariantCandidates.Add(node);
                        }
                        else if (node.Language == context.CurrentCulture.TwoLetterISOLanguageName)
                        {
                            objects.Add(node);
                            ignoreInvariantNodes = true;
                        }
                    }
                }
            }

            if (!ignoreInvariantNodes)
            {
                objects.AddRange(invariantCandidates);
            }

            return objects;
        } 
    }
}