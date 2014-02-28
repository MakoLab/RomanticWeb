using System;
using System.Diagnostics;

namespace RomanticWeb.Mapping.Model
{
    [DebuggerDisplay("Collection {Name}")]
    internal class CollectionMapping : PropertyMapping, IPropertyMapping
    {
        private readonly StorageStrategyOption _storageStrategy;

        public CollectionMapping(Type returnType,string name,Uri predicateUri,StorageStrategyOption storageStrategy)
            :base(returnType,name,predicateUri)
        {
            _storageStrategy=storageStrategy;
        }

        bool IPropertyMapping.IsCollection
        {
            get
            {
                return true;
            }
        }

        StorageStrategyOption IPropertyMapping.StorageStrategy
        {
            get
            {
                return _storageStrategy;
            }
        }
    }
}