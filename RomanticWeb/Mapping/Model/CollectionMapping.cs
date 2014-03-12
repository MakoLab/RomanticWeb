using System;
using System.Diagnostics;
using NullGuard;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Collection {Name}")]
    internal class CollectionMapping:PropertyMapping,ICollectionMapping
    {
        public CollectionMapping(Type returnType,string name,Uri predicateUri,StorageStrategyOption storageStrategy)
            :base(returnType,name,predicateUri)
        {
            StorageStrategy=storageStrategy;
        }

        public StorageStrategyOption StorageStrategy { get; set; }
    }
}