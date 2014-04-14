using System;
using System.Diagnostics;
using NullGuard;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Collection {Name}")]
    internal class CollectionMapping:PropertyMapping,ICollectionMapping
    {
        public CollectionMapping(Type declaringType,Type returnType,string name,Uri predicateUri,StoreAs storageStrategy):base(declaringType,returnType,name,predicateUri)
        {
            StoreAs=storageStrategy;
        }

        public StoreAs StoreAs { get; private set; }

        void IPropertyMapping.Accept(IMappingModelVisitor mappingModelVisitor)
        {
            mappingModelVisitor.Visit(this);
        }
    }
}