using System;

namespace RomanticWeb.Mapping.Model
{
    internal class PropertyMapping:IPropertyMapping
    {
        public PropertyMapping(Type returnType,string name,Uri predicateUri,IGraphSelectionStrategy graphSelector)
        {
            if (returnType == null) { throw new ArgumentNullException("returnType"); }
            if (name == null) { throw new ArgumentNullException("name"); }
            if (predicateUri == null) { throw new ArgumentNullException("predicateUri"); }

            ReturnType=returnType;
            Name=name;
            Uri=predicateUri;
            GraphSelector=graphSelector??new DefaultGraphSelector();
        }

        public Uri Uri { get; private set; }

        public IGraphSelectionStrategy GraphSelector { get; private set; }

        public string Name { get; private set; }

        public bool IsCollection
        {
            get
            {
                return false;
            }
        }

        public Type ReturnType { get; private set; }

        public StorageStrategyOption StorageStrategy
        {
            get
            {
                return StorageStrategyOption.None;
            }
        }
    }
}
