using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace RomanticWeb.JsonLd
{
    internal static class JsonExtensions
    {
        internal static JObject Merge(this JObject current, JObject toMerge)
        {
            foreach (var property in toMerge.Properties())
            {
                var currentValue = current[property.Name];

                if (currentValue == null)
                {
                    current[property.Name] = property.Value;
                    continue;
                }

                if (property.Name == "@graph")
                {
                    foreach (var element in property.Value)
                    {
                        ((JArray)currentValue).Add(element);
                    }
                }
            }

            return current;
        }

        internal static bool IsPropertySet(this JObject @object, string propertyName)
        {
            return @object.Property(propertyName) != null;
        }

        internal static int PropertyCount(this JObject @object)
        {
            return @object.Properties().Count();
        }

        internal static int PropertyCount(this JObject @object, Func<JProperty, bool> predicate)
        {
            return @object.Properties().Count(predicate);
        }

        internal static bool ValueEquals(this JProperty property, object value)
        {
            JValue propertyValue = (property != null ? property.Value as JValue : null);
            return propertyValue.ValueEquals(value);
        }

        internal static bool ValueEquals(this JValue propertyValue, object value)
        {
            if (value == null)
            {
                return (propertyValue == null) || ((propertyValue != null) && (propertyValue.Value == null));
            }

            switch (value.GetType().FullName)
            {
                case "System.String":
                    return (propertyValue.Type != JTokenType.String ? false : (string)propertyValue.Value == (string)value);
                case "System.Boolean":
                    return (propertyValue.Type != JTokenType.Boolean ? false : (bool)propertyValue.Value == (bool)value);
                case "System.SByte":
                    return (propertyValue.Type != JTokenType.Integer ? false : (sbyte)(int)propertyValue.Value == (sbyte)value);
                case "System.Byte":
                    return (propertyValue.Type != JTokenType.Integer ? false : (byte)(int)propertyValue.Value == (byte)value);
                case "System.Int16":
                    return (propertyValue.Type != JTokenType.Integer ? false : (short)(int)propertyValue.Value == (short)value);
                case "System.UInt16":
                    return (propertyValue.Type != JTokenType.Integer ? false : (ushort)(int)propertyValue.Value == (ushort)value);
                case "System.Int32":
                    return (propertyValue.Type != JTokenType.Integer ? false : (int)propertyValue.Value == (int)value);
                case "System.UInt32":
                    return (propertyValue.Type != JTokenType.Integer ? false : (uint)(int)propertyValue.Value == (uint)value);
                case "System.Int64":
                    return (propertyValue.Type != JTokenType.Integer ? false : (long)(int)propertyValue.Value == (long)value);
                case "System.UInt64":
                    return (propertyValue.Type != JTokenType.Integer ? false : (long)(int)propertyValue.Value == (long)value);
                case "System.Single":
                    return (propertyValue.Type == JTokenType.Integer ? (float)(int)propertyValue.Value == (float)value : (propertyValue.Type == JTokenType.Float ? (float)propertyValue.Value == (float)value : false));
                case "System.Double":
                    return (propertyValue.Type == JTokenType.Integer ? (double)(int)propertyValue.Value == (double)value : (propertyValue.Type == JTokenType.Float ? (double)(float)propertyValue.Value == (double)value : false));
                case "System.Decimal":
                    return (propertyValue.Type == JTokenType.Integer ? (decimal)(int)propertyValue.Value == (decimal)value : (propertyValue.Type == JTokenType.Float ? (decimal)(float)propertyValue.Value == (decimal)value : false));
                default:
                    return false;
            }
        }

        internal static bool ValueIs<T>(this JProperty property)
        {
            JValue value = (property != null ? property.Value as JValue : null);
            return value.ValueIs<T>();
        }

        internal static bool ValueIs<T>(this JValue value)
        {
            ValidateType(typeof(T));
            if ((value == null) || (value.Value == null))
            {
                return false;
            }

            switch (typeof(T).FullName)
            {
                case "System.DateTime":
                    DateTime dateTime;
                    return (value.Type == JTokenType.Date) || ((value.Type == JTokenType.String) && (DateTime.TryParse((string)value.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)));
                case "System.TimeSpan":
                    return value.Type == JTokenType.TimeSpan;
                case "System.String":
                    return value.Type == JTokenType.String;
                case "System.Boolean":
                    return value.Type == JTokenType.Boolean;
                case "System.SByte":
                case "System.Byte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                    return value.Type == JTokenType.Integer;
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    return (value.Type == JTokenType.Float) || (value.Type == JTokenType.Integer);
                case "System.Uri":
                    Uri uri;
                    string uriString;
                    return (value.Type == JTokenType.Uri) || ((value.Type == JTokenType.String) && (Regex.IsMatch(uriString = value.ValueAs<string>(), "[a-zA-Z0-9]+://.+")) &&
                        (Uri.TryCreate(uriString, UriKind.Absolute, out uri)) && (uri != null));
            }

            return false;
        }

        internal static T ValueAs<T>(this JProperty property)
        {
            JValue value = (property != null ? property.Value as JValue : null);
            return value.ValueAs<T>();
        }

        internal static T ValueAs<T>(this JValue value)
        {
            ValidateType(typeof(T));
            if ((value == null) || (value.Value == null))
            {
                return default(T);
            }

            if (!value.ValueIs<T>())
            {
                throw new InvalidCastException(System.String.Format("Cannot cast '{0}' as '{1}'.", value.Value.GetType(), typeof(T)));
            }

            switch (typeof(T).FullName)
            {
                case "System.DateTime":
                    return (T)(object)(value.Type == JTokenType.String ? Convert.ToDateTime((string)value.Value) : value.Value);
                case "System.TimeSpan":
                    return (T)(object)value.Value;
                case "System.String":
                    return (T)(object)value.Value.ToString();
                case "System.Boolean":
                    return (T)(object)value.Value;
                case "System.SByte":
                    return (T)(object)Convert.ToSByte((long)value.Value);
                case "System.Byte":
                    return (T)(object)Convert.ToByte((long)value.Value);
                case "System.Int16":
                    return (T)(object)Convert.ToInt16((long)value.Value);
                case "System.UInt16":
                    return (T)(object)Convert.ToUInt16((long)value.Value);
                case "System.Int32":
                    return (T)(object)Convert.ToInt32((long)value.Value);
                case "System.UInt32":
                    return (T)(object)Convert.ToUInt32((long)value.Value);
                case "System.Int64":
                    return (T)value.Value;
                case "System.UInt64":
                    return (T)(object)Convert.ToUInt64((long)value.Value);
                case "System.Single":
                    return (T)(object)Convert.ToSingle((value.Type == JTokenType.Integer ? (long)value.Value : (double)value.Value));
                case "System.Double":
                    return (T)((value.Type == JTokenType.Integer ? (object)Convert.ToDouble((long)value.Value) : (double)value.Value));
                case "System.Decimal":
                    return (T)(object)Convert.ToDecimal((value.Type == JTokenType.Integer ? (long)value.Value : (double)value.Value));
                case "System.Uri":
                    return (value.Type == JTokenType.Uri ? (T)value.Value : (T)(object)new Uri(value.ValueAs<string>()));
                default:
                    throw new InvalidOperationException(System.String.Format("Cannot cast '{0}' as '{1}'.", value.Value, typeof(T)));
            }
        }

        internal static JArray AsArray(this JToken token)
        {
            return (!(token is JArray) ? new JArray(token) : (JArray)token);
        }

        internal static JArray Merge(this JArray current, JToken toMerge)
        {
            if (toMerge is JArray)
            {
                JArray array = (JArray)toMerge;
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

        private static void ValidateType(Type type)
        {
            if ((type != typeof(Uri)) && (type != typeof(string)))
            {
                if (!type.IsValueType)
                {
                    throw new InvalidOperationException(System.String.Format("Cannot confirm value of non-value type '{0}'.", type));
                }

                if ((type != typeof(int)) && (type != typeof(byte)) && (type != typeof(long)) && (type != typeof(short)) &&
                    (type != typeof(float)) && (type != typeof(double)) && (type != typeof(decimal)) && (type != typeof(bool)) &&
                    (type != typeof(DateTime)) && (type != typeof(TimeSpan)))
                {
                    throw new InvalidOperationException(System.String.Format("Cannot confirm value of type '{0}'.", type));
                }
            }
        }
    }
}