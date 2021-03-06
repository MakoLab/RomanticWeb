﻿using System.Linq;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    internal static class CatalogConverterExtensions
    {
        public static ILiteralNodeConverter GetBestConverter(this IConverterCatalog catalog, Node literalNode)
        {
            var matches = from converter in catalog.LiteralNodeConverters
                          let match = converter.CanConvert(literalNode)
                          where match.LiteralFormatMatches != MatchResult.NoMatch
                                && match.DatatypeMatches != MatchResult.NoMatch
                          orderby match
                          select converter;

            return matches.FirstOrDefault();
        }
    }
}