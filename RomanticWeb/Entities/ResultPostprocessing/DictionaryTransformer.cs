using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Collections;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    public class DictionaryTransformer:IResultTransformer
    {
        private readonly IDictionaryPairTypeProvider _typeProvider;

        public DictionaryTransformer(IDictionaryPairTypeProvider typeProvider)
        {
            _typeProvider=typeProvider;
        }

        public DictionaryTransformer()
            :this(new DynamicAssemblyProvider())
        {
        }

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

        public void SetTransformed(object value,IEntityStore store)
        {
            throw new System.NotImplementedException();
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