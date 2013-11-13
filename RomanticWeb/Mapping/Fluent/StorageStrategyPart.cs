using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    public class StorageStrategyPart
    {
        private readonly CollectionMap _collectionMap;

        public StorageStrategyPart(CollectionMap collectionMap)
        {
            _collectionMap=collectionMap;
        }

        public CollectionMap SimpleCollection()
        {
            _collectionMap.StorageStrategy = StorageStrategyOption.Simple;
            return _collectionMap;
        }

        public CollectionMap RdfList()
        {
            _collectionMap.StorageStrategy = StorageStrategyOption.RdfList;
            return _collectionMap;
        }
    }
}