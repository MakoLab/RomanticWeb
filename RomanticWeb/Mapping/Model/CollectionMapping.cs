using System;
using System.Diagnostics;
using NullGuard;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Collection {Name}")]
    internal class CollectionMapping:PropertyMapping
    {
        public CollectionMapping(Type returnType,string name,Uri predicateUri,StorageStrategyOption storageStrategy)
            :base(returnType,name,predicateUri)
        {
            StorageStrategy=storageStrategy;
        }

        public override StorageStrategyOption StorageStrategy { get; set; }
    }
}