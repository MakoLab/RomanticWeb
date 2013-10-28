namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using Extensions;
    using Json;

    /// <summary>
    /// Deserializes request bodies in JSON format
    /// </summary>
    public class JsonBodyDeserializer : IBodyDeserializer
    {
        private readonly MethodInfo deserializeMethod = typeof(JavaScriptSerializer).GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            return Json.IsJsonContentType(contentType);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            var serializer = new JavaScriptSerializer(null, false, JsonSettings.MaxJsonLength, JsonSettings.MaxRecursions);
            serializer.RegisterConverters(JsonSettings.Converters);

            bodyStream.Position = 0;
            string bodyText;
            using (var bodyReader = new StreamReader(bodyStream))
            {
                bodyText = bodyReader.ReadToEnd();
            }

            var genericDeserializeMethod = this.deserializeMethod.MakeGenericMethod(new[] { context.DestinationType });

            var deserializedObject = genericDeserializeMethod.Invoke(serializer, new[] { bodyText });

            return deserializedObject;
        }

        private static object ConvertCollection(object items, Type destinationType, BindingContext context)
        {
            var returnCollection = Activator.CreateInstance(destinationType);

            var collectionAddMethod = 
                destinationType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in (IEnumerable)items)
            {
                collectionAddMethod.Invoke(returnCollection, new[] { item });
            }

            return returnCollection;
        }

        private static object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType);

            if (context.DestinationType.IsCollection())
            {
                return ConvertCollection(deserializedObject, context.DestinationType, context);
            }

            foreach (var property in context.ValidModelProperties)
            {
                CopyPropertyValue(property, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private static void CopyPropertyValue(PropertyInfo property, object sourceObject, object destinationObject)
        {
            property.SetValue(destinationObject, property.GetValue(sourceObject, null), null);
        }
    }
}