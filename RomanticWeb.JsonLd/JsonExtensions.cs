using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace RomanticWeb.JsonLd
{
    internal static class JsonExtensions
    {
        internal static JObject Merge(this JObject current,JObject toMerge)
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

        internal static bool ValueEquals(this JProperty property,object value)
        {
            if (property==null)
            {
                return false;
            }

            if (value==null)
            {
                return (property.Value==null)||((property.Value is JValue)&&(((JValue)property.Value).Value==null));
            }

            if (value is string)
            {
                if (!(property.Value is JValue)) { return false; }
                if (property.Value.Type!=JTokenType.String) { return false; }
                return (string)((JValue)property.Value).Value==(string)value;
            }

            if ((value is int)||(value is short)||(value is long)||(value is byte))
            {
                if (!(property.Value is JValue)) { return false; }
                if (property.Value.Type!=JTokenType.Integer) { return false; }
                if (value is int) { return (int)((JValue)property.Value).Value==(int)value; }
                if (value is short) { return (short)(int)((JValue)property.Value).Value==(int)value; }
                if (value is long) { return (long)(int)((JValue)property.Value).Value==(int)value; }
                if (value is byte) { return (byte)(int)((JValue)property.Value).Value==(int)value; }
            }

            if ((value is float)||(value is double)||(value is decimal))
            {
                if (!(property.Value is JValue)) { return false; }
                if (property.Value.Type==JTokenType.Float)
                {
                    if (value is float) { return (float)(double)((JValue)property.Value).Value==(int)value; }
                    if (value is double) { return (double)((JValue)property.Value).Value==(int)value; }
                    if (value is decimal) { return (decimal)(double)((JValue)property.Value).Value==(int)value; }
                }
                else if (property.Value.Type==JTokenType.Integer)
                {
                    if (value is float) { return (float)(int)((JValue)property.Value).Value==(int)value; }
                    if (value is double) { return (double)(int)((JValue)property.Value).Value==(int)value; }
                    if (value is decimal) { return (decimal)(int)((JValue)property.Value).Value==(int)value; }
                }
            }

            if (value is bool)
            {
                if (!(property.Value is JValue)) { return false; }
                if (property.Value.Type!=JTokenType.Boolean) { return false; }
                return (bool)((JValue)property.Value).Value==(bool)value;
            }

            return false;
        }

        internal static bool ValueIs<T>(this JProperty property)
        {
            if ((typeof(T)!=typeof(Uri))&&(typeof(T)!=typeof(string)))
            {
                if (!typeof(T).IsValueType)
                {
                    throw new InvalidOperationException(System.String.Format("Cannot confirm value of non-value type '{0}'.",typeof(T)));
                }

                if ((typeof(T)!=typeof(int))&&(typeof(T)!=typeof(byte))&&(typeof(T)!=typeof(long))&&(typeof(T)!=typeof(short))&&
                    (typeof(T)!=typeof(float))&&(typeof(T)!=typeof(double))&&(typeof(T)!=typeof(decimal))&&(typeof(T)!=typeof(bool)))
                {
                    throw new InvalidOperationException(System.String.Format("Cannot confirm value of type '{0}'.",typeof(T)));
                }
            }

            if ((property==null)||(property.Value==null)||(!(property.Value is JValue)))
            {
                return false;
            }

            if (typeof(T)==typeof(string))
            {
                return (((JValue)property.Value).Type==JTokenType.String);
            }

            if ((typeof(T)==typeof(int))||(typeof(T)==typeof(byte))||(typeof(T)==typeof(long))||(typeof(T)==typeof(short)))
            {
                return (((JValue)property.Value).Type==JTokenType.Integer);
            }

            if ((typeof(T)==typeof(float))||(typeof(T)==typeof(double))||(typeof(T)==typeof(decimal))||(typeof(T)==typeof(short)))
            {
                return ((((JValue)property.Value).Type==JTokenType.Integer)||(((JValue)property.Value).Type==JTokenType.Float));
            }

            if (typeof(T)==typeof(bool))
            {
                return (((JValue)property.Value).Type==JTokenType.Boolean);
            }

            if (typeof(T)==typeof(Uri))
            {
                Uri uri;
                return ((((JValue)property.Value).Type==JTokenType.String)&&(Regex.IsMatch(property.ValueAs<string>(),"[a-zA-Z0-9]+://.+"))&&
                    (Uri.TryCreate(property.ValueAs<string>(),UriKind.Absolute,out uri))&&(uri!=null))||(((JValue)property.Value).Type==JTokenType.Uri);
            }

            return false;
        }

        internal static T ValueAs<T>(this JProperty property)
        {
            if ((typeof(T)!=typeof(Uri))&&(typeof(T)!=typeof(string)))
            {
                if (!typeof(T).IsValueType)
                {
                    throw new InvalidCastException(System.String.Format("Cannot cast value to non-value type '{0}'.",typeof(T)));
                }

                if ((typeof(T)!=typeof(int))&&(typeof(T)!=typeof(byte))&&(typeof(T)!=typeof(long))&&(typeof(T)!=typeof(short))&&
                    (typeof(T)!=typeof(float))&&(typeof(T)!=typeof(double))&&(typeof(T)!=typeof(decimal))&&(typeof(T)!=typeof(bool)))
                {
                    throw new InvalidCastException(System.String.Format("Cannot cast value to type '{0}'.",typeof(T)));
                }
            }

            if (!(property.Value is JValue))
            {
                return default(T);
            }

            if ((property==null)||(property.Value==null)||(((JValue)property.Value).Value==null))
            {
                return default(T);
            }

            if (typeof(T)==typeof(string))
            {
                if (((JValue)property.Value).Type!=JTokenType.String)
                {
                    throw new InvalidCastException(System.String.Format("Cannot cast '{0}' as '{1}'.",((JValue)property.Value).Value.GetType(),typeof(T)));
                }

                return (T)((JValue)property.Value).Value;
            }

            if (((typeof(T)==typeof(int))||(typeof(T)==typeof(byte))||(typeof(T)==typeof(long))||(typeof(T)==typeof(short)))&&(((JValue)property.Value).Type==JTokenType.Integer))
            {
                return (T)((JValue)property.Value).Value;
            }

            if ((typeof(T)==typeof(float))||(typeof(T)==typeof(double))||(typeof(T)==typeof(decimal))||(typeof(T)==typeof(short)))
            {
                if (((JValue)property.Value).Type==JTokenType.Integer)
                {
                    return (T)(object)(decimal)(int)((JValue)property.Value).Value;
                }
                else if (((JValue)property.Value).Type==JTokenType.Float)
                {
                    return (T)((JValue)property.Value).Value;
                }
            }

            if ((typeof(T)==typeof(bool))&&(((JValue)property.Value).Type==JTokenType.Boolean))
            {
                return (T)((JValue)property.Value).Value;
            }

            if ((typeof(T)==typeof(Uri))&&(((JValue)property.Value).Type== JTokenType.Uri))
            {
                return (T)((JValue)property.Value).Value;
            }

            throw new InvalidOperationException(System.String.Format("Cannot cast value to type '{0}'.",typeof(T)));
        }

        internal static JArray Merge(this JArray current,JToken toMerge)
        {
            if (toMerge is JArray)
            {
                JArray array=(JArray)toMerge;
                foreach (JToken item in array)
                {
                    current.Add(item);
                }
            }
            else
            {
                current.Add(toMerge);
            }

            return current;
        }
    }
}