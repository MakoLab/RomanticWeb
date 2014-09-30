using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    #region Enums
    /// <summary>Enlists possible entity accessor sources.</summary>
    internal enum SourceTypes
    {
        /// <summary>Variable source.</summary>
        Variable,

        /// <summary>Member source.</summary>
        Member
    }
    #endregion

    /// <summary>Provides details about entity accessor.</summary>
    [QueryComponentNavigator(typeof(StrongEntityAccessorNavigator))]
    public class StrongEntityAccessor : QueryElement, ISelectableQueryComponent, IExpression
    {
        #region Fields
        private Identifier _about;
        private IList<QueryElement> _elements;
        private SourceTypes _source;
        private Remotion.Linq.Clauses.FromClauseBase _sourceExpression;
        #endregion

        #region Constructors
        /// <summary>Default constructor with aboutness assuming that a source is a variable.</summary>
        /// <param name="about">Points to the primary topic of given entity accessor.</param>
        internal StrongEntityAccessor(Identifier about)
            : this()
        {
            About = about;
            _source = SourceTypes.Variable;
            UnboundGraphName = null;
        }

        /// <summary>Default constructor with aboutness and its source passed.</summary>
        /// <param name="about">Specifies an entity identifier given accesor uses.</param>
        /// <param name="sourceExpression">Source of this entity accessor.</param>
        internal StrongEntityAccessor(Identifier about, Remotion.Linq.Clauses.FromClauseBase sourceExpression)
            : this()
        {
            About = about;
            _source = SourceTypes.Member;
            _sourceExpression = sourceExpression;
        }

        private StrongEntityAccessor()
            : base()
        {
            ObservableCollection<QueryElement> elements = new ObservableCollection<QueryElement>();
            elements.CollectionChanged += OnElementsCollectionChanged;
            _elements = elements;
        }
        #endregion

        #region Properties
        /// <summary>Gets a primary topic of this entity accessor.</summary>
        public Identifier About
        {
            get
            {
                return _about;
            }

            internal set
            {
                if ((_about = value) != null)
                {
                    _about.OwnerQuery = OwnerQuery;
                }
            }
        }

        /// <summary>Gets a list of entity accessor elements.</summary>
        public IList<QueryElement> Elements { get { return _elements; } }

        /// <summary>
        /// Gets or sets the name of the unbound graph.
        /// </summary>
        [AllowNull]
        public Identifier UnboundGraphName { [return: AllowNull] get; set; }

        /// <summary>Gets an enumeration of selectable expressions.</summary>
        IEnumerable<IExpression> ISelectableQueryComponent.Expressions { get { return new IExpression[] { this }; } }

        /// <summary>Gets a source type of this accessor.</summary>
        internal SourceTypes Source { get { return _source; } }

        /// <summary>Gets a source entity type of this accessor.</summary>
        internal Remotion.Linq.Clauses.FromClauseBase SourceExpression { get { return _sourceExpression; } }

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
                if (_about != null)
                {
                    _about.OwnerQuery = OwnerQuery;
                }

                foreach (QueryComponent queryComponent in _elements)
                {
                    queryComponent.OwnerQuery = value;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this graph.</summary>
        /// <returns>String representation of this graph.</returns>
        public override string ToString()
        {
            return System.String.Format(
                "GRAPH G{1} {0}{{{0}{2}{0}}}{0}GRAPH ?meta {{{0}G{1} foaf:primaryTopic {1} .}}{0}",
                Environment.NewLine,
                (_about != null ? _about.ToString() : System.String.Empty),
                System.String.Join(Environment.NewLine, _elements.Select(item => (item is StrongEntityAccessor ? (_about != null ? item.ToString().Replace("?s ", _about.ToString()) : item.ToString()) : item.ToString()))));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(StrongEntityAccessor)) &&
                (_about != null ? _about.Equals(((StrongEntityAccessor)operand)._about) : Object.Equals(((StrongEntityAccessor)operand)._about, null)) &&
                (_sourceExpression != null ? _sourceExpression.Equals(((StrongEntityAccessor)operand)._sourceExpression) : Object.Equals(((StrongEntityAccessor)operand)._sourceExpression, null)) &&
                (_source == ((StrongEntityAccessor)operand)._source);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(StrongEntityAccessor).FullName.GetHashCode() ^ (_about != null ? _about.GetHashCode() : 0) ^ (_sourceExpression != null ? _sourceExpression.GetHashCode() : 0) ^ _source.GetHashCode();
        }
        #endregion

        #region Non-public methods
        /// <summary>Rised when elements collection has changed.</summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Eventarguments.</param>
        protected void OnElementsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (QueryComponent queryComponent in e.NewItems)
                        {
                            if (queryComponent != null)
                            {
                                queryComponent.OwnerQuery = OwnerQuery;
                            }
                        }

                        break;
                    }
            }
        }
        #endregion
    }
}