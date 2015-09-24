using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NullGuard;
using RomanticWeb.ComponentModel;

namespace RomanticWeb.Entities
{
    /// <summary>An Entity's identifier (URI or blank node).</summary>
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    [TypeConverter(typeof(EntityIdTypeConverter<EntityId>))]
    public class EntityId : IComparable, IComparable<EntityId>, IXmlSerializable
    {
        private const string BlankScheme = "_";
        private int _hashCode = 0;
        private Uri _uri;

        /// <summary>Creates a new instance of <see cref="EntityId"/> from string.</summary>
        public EntityId(string uri) : this(new Uri(uri, UriKind.RelativeOrAbsolute))
        {
        }

        /// <summary>Creates a new instance of <see cref="EntityId"/> from an Uniform Resource Identifies.</summary>
        public EntityId(Uri uri)
        {
            _hashCode = (_uri = uri).ToString().GetHashCode();
        }

        /// <summary>Used for XML serialization.</summary>
        protected EntityId()
        {
        }

        /// <summary>The underlying Uniform Resource Identifier.</summary>
        public Uri Uri { get { return _uri; } }

        /// <summary>Tests for equality two entity identifiers.</summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><b>true</b> if both entity identifiers has same type and same Uri, otherwise <b>false</b>.</returns>
        public static bool operator ==([AllowNull] EntityId left, [AllowNull] EntityId right)
        {
            return Equals(left, right);
        }

        /// <summary>Tests for inequality two entity identifiers.</summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><b>true</b> if both entity identifiers are of different type or has different Uri, otherwise <b>false</b>.</returns>
        public static bool operator !=([AllowNull] EntityId left, [AllowNull] EntityId right)
        {
            return !(left == right);
        }

        /// <summary>Converts a string into an entity identifier.</summary>
        /// <param name="entityId">String representation of the entity identifier.</param>
        /// <returns><see cref="EntityId"/> instance or null.</returns>
        public static implicit operator EntityId(string entityId)
        {
            EntityId result = null;
            if (entityId != null)
            {
                result = new EntityId(entityId);
            }

            return result;
        }

        /// <summary>Converts an Uri into an entity identifier.</summary>
        /// <param name="uri">Uri representation of the entity identifier.</param>
        /// <returns><see cref="EntityId"/> instance or null.</returns>
        public static implicit operator EntityId(Uri uri)
        {
            EntityId result = null;
            if (uri != null)
            {
                result = new EntityId(uri);
            }

            return result;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// An object to compare with this object.</param>
        /// <returns>Type: <see cref="System.Int32" />
        /// A value that indicates the relative order of the objects being compared.</returns>
        int IComparable.CompareTo(object operand)
        {
            if (ReferenceEquals(operand, null))
            {
                return 1;
            }

            if (!(operand is EntityId))
            {
                throw new ArgumentException("operand");
            }

            var other = (EntityId)operand;
            if ((_uri.Scheme == BlankScheme) && (other._uri.Scheme == BlankScheme))
            {
                return _uri.ToString().CompareTo(other._uri.ToString());
            }

            if ((_uri.Scheme == BlankScheme) && (other._uri.Scheme != BlankScheme))
            {
                return -1;
            }

            if ((_uri.Scheme != BlankScheme) && (other._uri.Scheme == BlankScheme))
            {
                return 1;
            }

            return _uri.ToString().CompareTo(other.ToString());
        }

        /// <summary>Compares the current identifier with another identifier of the same type.</summary>
        /// <param name="other">Type: <see cref="EntityId" />
        /// An identifier to compare with this identifier.</param>
        /// <returns>Type: <see cref="System.Int32" />
        /// A value that indicates the relative order of the identifiers being compared.</returns>
        int IComparable<EntityId>.CompareTo(EntityId other)
        {
            return ((IComparable)this).CompareTo(other);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object obj)
        {
            if ((ReferenceEquals(obj, null)) || (GetType() != obj.GetType()))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return GetHashCode() == ((EntityId)obj).GetHashCode();
        }

        /// <summary>Creates a string representation of this entity identifier.</summary>
        /// <returns>String representation of this entity identifier.</returns>
        public override string ToString()
        {
            return _uri.ToString();
        }

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <param name="nQuadFormat">if set to <c>true</c> the string will be a valid NQuad node.</param>
        public virtual string ToString(bool nQuadFormat)
        {
            return ToString();
        }

        /// <summary>This method is reserved and should not be used.
        /// When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.</summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">Type: <see cref="System.Xml.XmlReader" />
        /// The <see cref="System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            _hashCode = (_uri = new Uri(reader.ReadElementContentAsString())).ToString().GetHashCode();
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">Type: <see cref="System.Xml.XmlWriter" />
        /// The <see cref="System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(_uri.ToString());
        }

        /// <summary>Determines whether the specified entity identifier is equal to the current object.</summary>
        /// <param name="other">Type: <see cref="EntityId" />
        /// The entity identifier to compare with the current entity identifier.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified identifier is equal to the current one; otherwise, <b>false</b>.</returns>
        protected bool Equals([AllowNull] EntityId other)
        {
            return Equals((object)other);
        }

        private class DebuggerViewProxy
        {
            private readonly EntityId _entityId;

            public DebuggerViewProxy(EntityId entityId)
            {
                _entityId = entityId;
            }

            public Uri Uri { get { return _entityId.Uri; } }
        }
    }
}