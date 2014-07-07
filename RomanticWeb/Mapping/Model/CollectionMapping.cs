using System;
using System.Diagnostics;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Collection {Name}")]
    internal class CollectionMapping : PropertyMapping, ICollectionMapping
    {
        public CollectionMapping(Type declaringType, Type returnType, string name, Uri predicateUri, StoreAs storageStrategy)
            : base(declaringType, returnType, name, predicateUri)
        {
            StoreAs = storageStrategy;
        }

        public StoreAs StoreAs { get; private set; }

        public INodeConverter ElementConverter { get; internal set; }

        void IPropertyMapping.Accept(IMappingModelVisitor mappingModelVisitor)
        {
            mappingModelVisitor.Visit(this);
        }
    }
}