using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides details about identifier entity accessor.</summary>
    [QueryComponentNavigator(typeof(IdentifierEntityAccessorNavigator))]
    public class IdentifierEntityAccessor : StrongEntityAccessor
    {
        #region Fields
        private StrongEntityAccessor _entityAccessor;
        #endregion

        #region Constructors
        /// <summary>Default constructor with aboutness and a strong entity accessor.</summary>
        /// <param name="about">Points to the primary topic of given entity accessor.</param>
        /// <param name="entityAccessor">Strong entity accessor.</param>
        internal IdentifierEntityAccessor(Identifier about, StrongEntityAccessor entityAccessor)
            : base(about)
        {
            _entityAccessor = entityAccessor;
        }

        /// <summary>Default constructor with aboutness and its source passed.</summary>
        /// <param name="about">Specifies an entity identifier given accesor uses.</param>
        /// <param name="sourceExpression">Source of this entity accessor.</param>
        /// <param name="entityAccessor">Strong entity accessor.</param>
        internal IdentifierEntityAccessor(Identifier about, Remotion.Linq.Clauses.FromClauseBase sourceExpression, StrongEntityAccessor entityAccessor)
            : base(about, sourceExpression)
        {
            _entityAccessor = entityAccessor;
        }
        #endregion

        #region Properties
        /// <summary>Gets a strong entity accessor.</summary>
        internal StrongEntityAccessor EntityAccessor { get { return _entityAccessor; } }

        /// <summary>Gets an owning query.</summary>
        internal override Query OwnerQuery
        {
            get
            {
                return base.OwnerQuery;
            }

            set
            {
                base.OwnerQuery = value;
                _entityAccessor.OwnerQuery = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this graph.</summary>
        /// <returns>String representation of this graph.</returns>
        public override string ToString()
        {
            IList<string> elements = Elements.Select(item =>
                    (item is StrongEntityAccessor ? (About != null ? item.ToString().Replace("?s ", About.ToString()) : (_entityAccessor.About != null ? _entityAccessor.About.ToString() : item.ToString())) :
                    item.ToString())).ToList();
            elements.Add(System.String.Format("BIND(<{0}> AS {1}Fake)", Rdf.predicate, (About != null ? About.ToString() : _entityAccessor.About != null ? _entityAccessor.About.ToString() : System.String.Empty)));
            elements.Add(System.String.Format("BIND(<{0}> AS {1}Fake)", Rdf.@object, (About != null ? About.ToString() : _entityAccessor.About != null ? _entityAccessor.About.ToString() : System.String.Empty)));

            return System.String.Format(
                "GRAPH G{1} {0}{{{0}{2}{0}}}{0}GRAPH ?meta {{{0}G{1} foaf:primaryTopic {1} .}}{0}",
                Environment.NewLine,
                (About != null ? About.ToString() : System.String.Empty),
                System.String.Join(Environment.NewLine, elements));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            if (Object.Equals(operand, null)) { return false; }
            if (operand.GetType() != typeof(IdentifierEntityAccessor)) { return false; }
            IdentifierEntityAccessor accessor = (IdentifierEntityAccessor)operand;
            return (About != null ? About.Equals(accessor.About) : Object.Equals(accessor.About, null)) &&
                (SourceExpression != null ? SourceExpression.Equals(accessor.SourceExpression) : Object.Equals(accessor.SourceExpression, null)) &&
                (Source == accessor.Source) && (_entityAccessor.Equals(accessor._entityAccessor));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(IdentifierEntityAccessor).FullName.GetHashCode() ^ (About != null ? About.GetHashCode() : 0) ^
                (SourceExpression != null ? SourceExpression.GetHashCode() : 0) ^ Source.GetHashCode() ^ _entityAccessor.GetHashCode();
        }
        #endregion
    }
}