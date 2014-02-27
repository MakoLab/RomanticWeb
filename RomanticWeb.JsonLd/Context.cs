using System;
using System.Collections;
using System.Collections.Generic;

namespace RomanticWeb.JsonLd
{
    internal class Context:IDictionary<string,TermDefinition>,ICloneable
    {
        #region Fields
        private readonly IDictionary<string,TermDefinition> _termDefinitions=new Dictionary<string,TermDefinition>();
        #endregion

        #region Properties

        #region ICollection<KeyValuePair<string,TermDefinition>> properties
        bool ICollection<KeyValuePair<string, TermDefinition>>.IsReadOnly { get { return _termDefinitions.IsReadOnly; } }

        int ICollection<KeyValuePair<string, TermDefinition>>.Count { get { return _termDefinitions.Count; } }
        #endregion

        #region IDictionary<string,TermDefinition> properties
        ICollection<string> IDictionary<string, TermDefinition>.Keys { get { return _termDefinitions.Keys; } }

        ICollection<TermDefinition> IDictionary<string, TermDefinition>.Values { get { return _termDefinitions.Values; } }
        #endregion

        public int TermsCount { get { return _termDefinitions.Count; } }

        public IEnumerable<string> TermNames { get { return _termDefinitions.Keys; } }

        public IEnumerable<TermDefinition> TermDefinitions { get { return _termDefinitions.Values; } }

        public Uri DocumentUri { get; set; }

        public string BaseIri { get; set; }

        public string Vocabulary { get; set; }

        public string Language { get; set; }

        public TermDefinition this[string termName]
        {
            get
            {
                return (_termDefinitions.ContainsKey(termName)?_termDefinitions[termName]:null);
            }

            set
            {
                _termDefinitions[termName]=value;
            }
        }
        #endregion

        #region Public methods
        public void Add(string termName,TermDefinition termDefinition)
        {
            _termDefinitions.Add(termName,termDefinition);
        }

        public bool ContainsKey(string termName)
        {
            return _termDefinitions.ContainsKey(termName);
        }

        public bool Remove(string termName)
        {
            return _termDefinitions.Remove(termName);
        }

        public bool TryGetValue(string termName,out TermDefinition termDefinition)
        {
            return _termDefinitions.TryGetValue(termName,out termDefinition);
        }

        public void Clear()
        {
            _termDefinitions.Clear();
        }

        public void Merge(Context localContext)
        {
            if (localContext!=null)
            {
                foreach (KeyValuePair<string,TermDefinition> term in localContext)
                {
                    if (!ContainsKey(term.Key))
                    {
                        this[term.Key]=term.Value;
                    }
                }
            }
        }

        #region ICollection<KeyValuePair<string,TermDefinition>> methods
        void ICollection<KeyValuePair<string,TermDefinition>>.Add(KeyValuePair<string,TermDefinition> term)
        {
            _termDefinitions.Add(term);
        }

        bool ICollection<KeyValuePair<string,TermDefinition>>.Contains(KeyValuePair<string,TermDefinition> item)
        {
            return _termDefinitions.Contains(item);
        }

        void ICollection<KeyValuePair<string,TermDefinition>>.CopyTo(KeyValuePair<string,TermDefinition>[] array,int arrayIndex)
        {
            _termDefinitions.CopyTo(array,arrayIndex);
        }

        bool ICollection<KeyValuePair<string,TermDefinition>>.Remove(KeyValuePair<string,TermDefinition> term)
        {
            return _termDefinitions.Remove(term);
        }
        #endregion

        public IEnumerator<KeyValuePair<string,TermDefinition>> GetEnumerator()
        {
            return _termDefinitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _termDefinitions.GetEnumerator();
        }

        public Context Clone()
        {
            var result=new Context();
            result.BaseIri=BaseIri;
            result.DocumentUri=DocumentUri;
            result.Vocabulary=Vocabulary;
            result.Language=Language;
            foreach (KeyValuePair<string,TermDefinition> term in this)
            {
                ((IDictionary<string,TermDefinition>)result).Add(new KeyValuePair<string,TermDefinition>(term.Key,term.Value.Clone()));
            }

            return result;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
    }
}