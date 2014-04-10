using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides details about entity accessor.</summary>
    [QueryComponentNavigator(typeof(UnspecifiedEntityAccessorNavigator))]
    public class UnspecifiedEntityAccessor:StrongEntityAccessor
    {
        #region Fields
        private StrongEntityAccessor _entityAccessor;
        #endregion

        #region Constructors
        /// <summary>Default constructor with aboutness assuming that a source is a variable.</summary>
        /// <param name="about">Points to the primary topic of given entity accessor.</param>
        /// <param name="entityAccessor">Strong entity accessor.</param>
        internal UnspecifiedEntityAccessor(Identifier about,StrongEntityAccessor entityAccessor):base(about)
        {
            _entityAccessor=entityAccessor;
        }

        /// <summary>Default constructor with aboutness and its source passed.</summary>
        /// <param name="about">Specifies an entity identifier given accesor uses.</param>
        /// <param name="sourceExpression">Source of this entity accessor.</param>
        /// <param name="entityAccessor">Strong entity accessor.</param>
        internal UnspecifiedEntityAccessor(Identifier about,Remotion.Linq.Clauses.FromClauseBase sourceExpression,StrongEntityAccessor entityAccessor):base(about,sourceExpression)
        {
            _entityAccessor=entityAccessor;
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
                base.OwnerQuery=value;
                _entityAccessor.OwnerQuery=value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this graph.</summary>
        /// <returns>String representation of this graph.</returns>
        public override string ToString()
        {
            IEnumerable<string> elements=Elements.Select(item => 
                    (item is StrongEntityAccessor?(About!=null?item.ToString().Replace("?s ",About.ToString()):(_entityAccessor.About!=null?_entityAccessor.About.ToString():item.ToString())):
                    item.ToString()));
            string strongEntityAccessor=_entityAccessor.ToString();
            foreach (IQueryComponent component in Elements)
            {
                strongEntityAccessor=strongEntityAccessor.Replace(component.ToString(),System.String.Empty);
            }

            return System.String.Format(
                "{3} UNION {{{0}GRAPH G{1} {0}{{{0}{2}{0}}}{0}GRAPH ?meta {{{0}G{1} foaf:primaryTopic {1} .}}{0}}}{0}",
                Environment.NewLine,
                (About!=null?About.ToString():(_entityAccessor.About!=null?_entityAccessor.About.ToString():System.String.Empty)),
                System.String.Join(Environment.NewLine,elements),
                strongEntityAccessor);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(UnspecifiedEntityAccessor))&&
                (About!=null?About.Equals(((UnspecifiedEntityAccessor)operand).About):Object.Equals(((UnspecifiedEntityAccessor)operand).About,null))&&
                (SourceExpression!=null?SourceExpression.Equals(((UnspecifiedEntityAccessor)operand).SourceExpression):Object.Equals(((UnspecifiedEntityAccessor)operand).SourceExpression,null))&&
                (Source==((UnspecifiedEntityAccessor)operand).Source)&&(_entityAccessor.Equals(((UnspecifiedEntityAccessor)operand)._entityAccessor));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(UnspecifiedEntityAccessor).FullName.GetHashCode()^(About!=null?About.GetHashCode():0)^
                (SourceExpression!=null?SourceExpression.GetHashCode():0)^Source.GetHashCode()^_entityAccessor.GetHashCode();
        }
        #endregion
    }
}