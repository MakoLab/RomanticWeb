using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace RomanticWeb.JsonLd
{
    internal static class ContextExtensions
    {
        internal static void SetupBase(this Context context, JObject localContext, IList<string> remoteContexts)
        {
            if ((localContext.IsPropertySet(JsonLdProcessor.Base)) && (remoteContexts.Count == 0))
            {
                string value = localContext.Property(JsonLdProcessor.Base).ValueAs<string>();
                if (value == null)
                {
                    context.BaseIri = null;
                }
                else if (Regex.IsMatch(value, "[a-zA-Z0-9_]+://.+"))
                {
                    context.BaseIri = value;
                }
                else if ((!Regex.IsMatch(value, "[a-zA-Z0-9_]+:.+")) && (context.BaseIri != null))
                {
                    context.BaseIri = JsonLdProcessor.MakeAbsoluteUri(context.BaseIri, value);
                }
                else
                {
                    throw new InvalidOperationException("Invalid base IRI.");
                }
            }
        }

        internal static void SetupVocab(this Context context, JObject localContext)
        {
            if (localContext.IsPropertySet(JsonLdProcessor.Vocab))
            {
                string value = localContext.Property(JsonLdProcessor.Vocab).ValueAs<string>();
                if (value == null)
                {
                    context.Vocabulary = null;
                }
                else if ((Regex.IsMatch(value, "[a-zA-Z0-9_]+://")) || (value.StartsWith("_:")))
                {
                    context.Vocabulary = value;
                }
                else
                {
                    throw new InvalidOperationException("Invalid vocab mapping.");
                }
            }
        }

        internal static void SetupLanguage(this Context context, JObject localContext)
        {
            if (localContext.IsPropertySet(JsonLdProcessor.Language))
            {
                if (localContext.Property(JsonLdProcessor.Language).ValueEquals(null))
                {
                    context.Language = null;
                }
                else if (!localContext.Property(JsonLdProcessor.Language).ValueIs<string>())
                {
                    throw new InvalidOperationException("Invalid default language.");
                }
                else
                {
                    context.Language = localContext.Property(JsonLdProcessor.Language).ValueAs<string>().ToLower();
                }
            }
        }
    }
}