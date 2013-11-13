using System;

namespace RomanticWeb.Mapping.Model
{
    internal class CollectionMapping : PropertyMapping, IPropertyMapping
    {
        private readonly StorageStrategyOption _storageStrategy;

        public CollectionMapping(Type returnType,string name,Uri predicateUri,IGraphSelectionStrategy graphSelector,StorageStrategyOption storageStrategy)
            :base(returnType,name,predicateUri,graphSelector)
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