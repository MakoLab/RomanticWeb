using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Property {Name}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    internal class PropertyMapping : IPropertyMapping
    {
        private readonly bool _isHidden;
        private readonly int _hashCode;

        public PropertyMapping(Type declaringType, Type returnType, string name, Uri predicateUri)
        {
            DeclaringType = declaringType;
            ReturnType = returnType;
            Name = name;
            Uri = predicateUri;
            _hashCode = CalculateHashCode();
        }

        public IEntityMapping EntityMapping { get; internal set; }

        public Uri Uri { get; private set; }

        public string Name { get; private set; }

        public Type DeclaringType { get; private set; }

        public Type ReturnType { get; private set; }

        public INodeConverter Converter { get; internal set; }

        public bool IsHidden { get { return _isHidden; } }

        public void Accept(IMappingModelVisitor mappingModelVisitor)
        {
            mappingModelVisitor.Visit(this);
        }

#pragma warning disable 1591
        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((ReferenceEquals(obj, null)) || (obj.GetType() != GetType()))
            {
                return false;
            }

            PropertyMapping other = (PropertyMapping)obj;
            return (DeclaringType == other.DeclaringType) && (ReturnType == other.ReturnType) && (Name.Equals(other.Name)) && (Uri.ToString().Equals(other.Uri.ToString()));
        }
#pragma warning restore

        private int CalculateHashCode()
        {
            unchecked
            {
                return 2978 * (DeclaringType.GetHashCode() ^ ReturnType.GetHashCode() ^ Name.GetHashCode() ^ Uri.ToString().GetHashCode());
            }
        }

        private class DebuggerViewProxy
        {
            private readonly PropertyMapping _mapping;

            public DebuggerViewProxy(PropertyMapping mapping)
            {
                _mapping = mapping;
            }

            public Uri Predicate { get { return _mapping.Uri; } }

            public string Name { get { return _mapping.Name; } }

            public Type ReturnType { get { return _mapping.ReturnType; } }

            public IEntityMapping EntityMapping { get { return _mapping.EntityMapping; } }
        }
    }
}
