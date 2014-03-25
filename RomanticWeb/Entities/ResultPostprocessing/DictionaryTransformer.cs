using System;
using System.Linq;
using RomanticWeb.Collections;
using RomanticWeb.Dynamic;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// Transforms the resulting nodes to a <see cref="RdfDictionary{TKey,TValue,TPair,TOwner}"/>
    /// </summary>
    public class DictionaryTransformer:IResultTransformer
    {
        private readonly IDictionaryTypeProvider _typeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTransformer"/> class.
        /// </summary>
        /// <param name="typeProvider">The type provider.</param>
        public DictionaryTransformer(IDictionaryTypeProvider typeProvider)
        {
            _typeProvider=typeProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTransformer"/> class.
        /// </summary>
        public DictionaryTransformer()
            :this(new DefaultDictionaryTypeProvider())
        {
        }

        /// <summary>
        /// Transforms the resulting nodes to a <see cref="RdfDictionary{TKey,TValue,TPair,TOwner}"/>
        /// </summary>
        public object GetTransformed(IEntityProxy parent,IPropertyMapping property,IEntityContext context,object value)
        {
            Type keyType=GetKeyType(property);
            Type valueType=GetValueType(property);
            Type pairEntityType=_typeProvider.GetEntryType(property);
            Type ownerType=_typeProvider.GetOwnerType(property);

            var constructor=typeof(RdfDictionary<,,,>)
                .MakeGenericType(keyType,valueType,pairEntityType,ownerType)
                .GetConstructors().Single();

            return constructor.Invoke(new object[] { parent.Id,context });
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void SetTransformed(object value,IEntityStore store)
        {
            throw new InvalidOperationException();
        }

        private static Type GetValueType(IPropertyMapping property)
        {
            return property.ReturnType.GenericTypeArguments[1];
        }

        private static Type GetKeyType(IPropertyMapping property)
        {
            return property.ReturnType.GenericTypeArguments[0];
        }
    }
}