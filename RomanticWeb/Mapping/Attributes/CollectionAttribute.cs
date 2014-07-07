using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a collection to an RDF predicate.</summary>
    public class CollectionAttribute : PropertyAttribute
    {
        private Type _elementConverterType = null;

        /// <summary>Initializes a new instance of the <see cref="CollectionAttribute"/> class.</summary>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="propertyName">Predicate name.</param>
        public CollectionAttribute(string prefix, string propertyName)
            : base(prefix, propertyName)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CollectionAttribute"/> class.</summary>
        /// <param name="propertyUri">The property URI.</param>
        public CollectionAttribute(string propertyUri)
            : base(propertyUri)
        {
        }

        /// <summary>Gets or sets the storage strategy.</summary>
        public StoreAs StoreAs { get; set; }

        /// <inheritdoc />
        public override Type ConverterType
        {
            [return: AllowNull]
            get { return (StoreAs == Model.StoreAs.SimpleCollection ? _elementConverterType ?? base.ConverterType : base.ConverterType); }
            set { base.ConverterType = value; }
        }

        /// <summary>Gets or sets an element converter type.</summary>
        public Type ElementConverterType
        {
            [return: AllowNull]
            get { return (StoreAs == Model.StoreAs.SimpleCollection ? _elementConverterType ?? base.ConverterType : _elementConverterType); }
            set { _elementConverterType = value; }
        }

        internal override IPropertyMappingProvider Accept(IMappingAttributesVisitor visitor, PropertyInfo property)
        {
            return visitor.Visit(this, property);
        }
    }
}