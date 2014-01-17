using System;
using System.Diagnostics;

namespace RomanticWeb.Mapping.Model
{
    [DebuggerDisplay("Property {Name}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    internal class PropertyMapping:IPropertyMapping
    {
        public PropertyMapping(Type returnType,string name,Uri predicateUri,GraphSelectionStrategyBase graphSelector)
        {
            if (returnType == null) { throw new ArgumentNullException("returnType"); }
            if (name == null) { throw new ArgumentNullException("name"); }
            if (predicateUri == null) { throw new ArgumentNullException("predicateUri"); }
            if (graphSelector == null) { throw new ArgumentNullException("graphSelector"); }

            ReturnType=returnType;
            Name=name;
            Uri=predicateUri;
            GraphSelector=graphSelector;
        }

        public Uri Uri { get; private set; }

        public GraphSelectionStrategyBase GraphSelector { get; private set; }

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

#pragma warning disable 1591
        public override string ToString()
        {
            return Name;
        }
#pragma warning restore

        private class DebuggerViewProxy
        {
            private readonly PropertyMapping _mapping;

            public DebuggerViewProxy(PropertyMapping mapping)
            {
                _mapping=mapping;
            }

            public Uri Predicate
            {
                get
                {
                    return _mapping.Uri;
                }
            }

            public string Name
            {
                get
                {
                    return _mapping.Name;
                }
            }

            public Type ReturnType
            {
                get
                {
                    return _mapping.ReturnType;
                }
            }
        }
    }
}
