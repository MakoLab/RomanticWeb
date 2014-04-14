using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Specifies target SPARQL query form.</summary>
    public enum QueryForms
    {
        /// <summary>Selects expressions in to the resulting data set.</summary>
        Select,

        /// <summary>Asks for existance of a given entities.</summary>
        Ask,

        /// <summary>Returns triples describing given entities.</summary>
        Describe,

        /// <summary>Returns triples that can be used to construct another triple store.</summary>
        Construct
    }

    /// <summary>Represents a whole query.</summary>
    [QueryComponentNavigator(typeof(QueryNavigator))]
    public class Query:QueryElement,IExpression
    {
        #region Fieds
        private IList<Prefix> _prefixes;
        private IList<ISelectableQueryComponent> _select;
        private IList<QueryElement> _elements;
        private IVariableNamingStrategy _variableNamingStrategy;
        private IVariableNamingConvention _variableNamingConvention;
        private Identifier _subject;
        private QueryForms _queryForm;
        private int _offset=-1;
        private int _limit=-1;
        private IDictionary<IExpression,bool> _orderBy=new Dictionary<IExpression,bool>();
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor</summary>
        internal Query():this(null)
        {
        }

        /// <summary>Constrctor with subject passed.</summary>
        /// <param name="subject">Subject of this query.</param>
        internal Query(Identifier subject):base()
        {
            _queryForm=QueryForms.Select;
            ObservableCollection<Prefix> prefixes=new ObservableCollection<Prefix>();
            prefixes.CollectionChanged+=OnCollectionChanged;
            _prefixes=prefixes;
            ObservableCollection<ISelectableQueryComponent> select=new ObservableCollection<ISelectableQueryComponent>();
            select.CollectionChanged+=OnCollectionChanged;
            _select=select;
            ObservableCollection<QueryElement> elements=new ObservableCollection<QueryElement>();
            elements.CollectionChanged+=OnCollectionChanged;
            _elements=elements;
            _variableNamingStrategy=new UniqueVariableNamingStrategy(this);
            _variableNamingConvention=new CamelCaseVariableNamingConvention();
            if ((_subject=subject)!=null)
            {
                _subject.OwnerQuery=this;
            }
        }

        /// <summary>Constructor with subject and variable naming strategy passed.</summary>
        /// <param name="subject">Subject of this query.</param>
        /// <param name="variableNamingStrategy">Varialbe naming strategy.</param>
        internal Query(Identifier subject,IVariableNamingStrategy variableNamingStrategy):this(subject)
        {
            _variableNamingStrategy=variableNamingStrategy;
            _variableNamingConvention=new CamelCaseVariableNamingConvention();
        }
        #endregion

        #region Properties
        /// <summary>Gets an enumeration of all prefixes.</summary>
        public IList<Prefix> Prefixes { get { return _prefixes; } }

        /// <summary>Gets an enumeration of all selected expressions.</summary>
        public IList<ISelectableQueryComponent> Select { get { return _select; } }

        /// <summary>Gets an enumeration of all query elements.</summary>
        public IList<QueryElement> Elements { get { return _elements; } }

        /// <summary>Gets a value indicating if the given query is actually a sub query.</summary>
        public bool IsSubQuery { get { return (OwnerQuery!=null); } }

        /// <summary>Gets a query form of given query.</summary>
        public QueryForms QueryForm { get { return _queryForm; } internal set { _queryForm=value; } }

        /// <summary>Gets or sets the offset.</summary>
        public int Offset { get { return _offset; } set { _offset=(value>=0?value:-1); } }

        /// <summary>Gets or sets the limit.</summary>
        public int Limit { get { return _limit; } set { _limit=(value>=0?value:-1); } }

        /// <summary>Gets a map of order by clauses.</summary>
        /// <remarks>Key is the expression on which the sorting should be performed and the value determines the direction, where <b>true</b> means descending and <b>false</b> is for ascending (default).</remarks>
        public IDictionary<IExpression,bool> OrderBy { get { return _orderBy; } }

        /// <summary>Gets an owning query.</summary>
        internal override Query OwnerQuery
        {
            [return: AllowNull]
            get
            {
                return base.OwnerQuery;
            }

            set
            {
                if (value!=null)
                {
                    base.OwnerQuery=value;
                }
            }
        }

        /// <summary>Subject of this query.</summary>
        [AllowNull]
        internal Identifier Subject
        {
            get
            {
                return _subject;
            }

            set
            {
                if (value==null)
                {
                    throw new ArgumentNullException("subject");
                }

                (_subject=value).OwnerQuery=this;
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a new blank query that can act as a sub query for this instance.</summary>
        /// <param name="subject">Primary subject of the resulting query.</param>
        /// <remarks>This method doesn't add the resulting query as a sub query of this instance.</remarks>
        /// <returns>Query that can act as a sub query for this instance.</returns>
        public Query CreateSubQuery(Identifier subject)
        {
            Query result=new Query(subject,_variableNamingStrategy);
            result.OwnerQuery=this;
            return result;
        }

        /// <summary>Creates a variable name from given identifier.</summary>
        /// <param name="identifier">Identifier to be used to abbreviate variable name.</param>
        /// <returns>Variable name with unique name.</returns>
        public string CreateVariableName(string identifier)
        {
            return _variableNamingStrategy.GetNameForIdentifier(CreateIdentifier(identifier));
        }

        /// <summary>Retrieves an identifier from a passed variable name.</summary>
        /// <param name="variableName">Variable name to retrieve identifier from.</param>
        /// <returns>Identifier passed to create the variable name.</returns>
        public string RetrieveIdentifier(string variableName)
        {
            return _variableNamingStrategy.ResolveNameToIdentifier(variableName);
        }

        /// <summary>Creates an identifier from given name.</summary>
        /// <param name="name">Name.</param>
        /// <returns>Identifier created from given name.</returns>
        public string CreateIdentifier(string name)
        {
            return _variableNamingConvention.GetIdentifierForName(name);
        }

        /// <summary>Creates a string representation of this query.</summary>
        /// <returns>String representation of this query.</returns>
        public override string ToString()
        {
            return System.String.Format(
                "{3} SELECT {1} {0}WHERE {0}{{{0}{2}{0}}}",
                Environment.NewLine,
                System.String.Join(" ",_select.Select(item => (item is StrongEntityAccessor?System.String.Format("?G{0} ?{0}",((StrongEntityAccessor)item).About.Name):item.ToString()))),
                System.String.Join(Environment.NewLine,_elements),
                System.String.Join(Environment.NewLine,_prefixes));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(Query))&&(_queryForm==((Query)operand)._queryForm)&&
                (_subject!=null?_subject.Equals(((Query)operand)._subject):Object.Equals(((Query)operand)._subject,null))&&
                (_variableNamingStrategy==((Query)operand)._variableNamingStrategy)&&
                (_prefixes.Count==((Query)operand)._prefixes.Count)&&
                (_select.Count==((Query)operand)._select.Count)&&
                (_elements.Count==((Query)operand)._elements.Count)&&
                (_prefixes.Count==((Query)operand)._prefixes.Count)&&
                (_prefixes.SequenceEqual(((Query)operand)._prefixes))&&
                (_select.SequenceEqual(((Query)operand)._select))&&
                (_elements.SequenceEqual(((Query)operand)._elements));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result=typeof(Query).FullName.GetHashCode()^_variableNamingStrategy.GetHashCode()^_queryForm.GetHashCode()^(_subject!=null?_subject.GetHashCode():0);
            foreach (Prefix prefix in _prefixes)
            {
                result^=prefix.GetHashCode();
            }

            foreach (ISelectableQueryComponent queryComponent in _select)
            {
                result^=queryComponent.GetHashCode();
            }

            foreach (IQueryComponent queryComponent in _elements)
            {
                result^=queryComponent.GetHashCode();
            }

            return result;
        }
        #endregion

        #region Non-public methods
        /// <summary>Rised when arguments collection has changed.</summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Eventarguments.</param>
        protected virtual void OnCollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (QueryComponent queryComponent in e.NewItems)
                        {
                            if (queryComponent!=null)
                            {
                                queryComponent.OwnerQuery=this;
                            }
                        }

                        break;
                    }
            }
        }
        #endregion
    }
}