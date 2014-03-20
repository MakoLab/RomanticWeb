using System;
using System.Diagnostics;
using NullGuard;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Collection {Name}")]
    internal class CollectionMapping:PropertyMapping,ICollectionMapping
    {
        public CollectionMapping(Type returnType,string name,Uri predicateUri,StoreAs storageStrategy)
            :base(returnType,name,predicateUri)
        {
            StoreAs=storageStrategy;
        }

        public StoreAs StoreAs { get; private set; }
    }
}