using System;
using System.Collections.Generic;
using RomanticWeb.Linq.Model;

namespace RomanticWeb.Linq
{
    /// <summary>Provides constistent and non-coliding names for identifiers.</summary>
    internal class UniqueVariableNamingStrategy:IVariableNamingStrategy
    {
        #region Fields
        private static Dictionary<Query,Dictionary<string,int>> NextIdentifiers=new Dictionary<Query,Dictionary<string,int>>();
        private static object _lock=new object();
        private Query _context;
        #endregion

        #region Constructors
        /// <summary>Default constructor with query context provided.</summary>
        /// <param name="context">Query context to be used as name distinction mechanism.</param>
        internal UniqueVariableNamingStrategy(Query context)
        {
            _context=context;
        }
        #endregion

        #region Destructors
        ~UniqueVariableNamingStrategy()
        {
            if (NextIdentifiers.ContainsKey(_context))
            {
                NextIdentifiers.Remove(_context);
            }
        }
        #endregion

        #region Public methods
        /// <summary>Gets a variable name for given identifier.</summary>
        /// <param name="identifier">Base identifier for which the name must be created.</param>
        /// <returns>Name of the variale coresponding for given identifier.</returns>
        public string GetNameForIdentifier(string identifier)
        {
            if (identifier.Length==0)
            {
                throw new InvalidOperationException("Cannot create a variable name for empty identifier.");
            }

            string result=identifier;
            lock (_lock)
            {
                if (!NextIdentifiers.ContainsKey(_context))
                {
                    NextIdentifiers[_context]=new Dictionary<string,int>();
                }

                if (!NextIdentifiers[_context].ContainsKey(identifier))
                {
                    NextIdentifiers[_context][identifier]=0;
                }

                result+=(NextIdentifiers[_context][identifier]++).ToString();
            }

            return result;
        }

        /// <summary>Reverses the process and resolves an initial identifier passed to create a variable name.</summary>
        /// <param name="name">Name to be resolved.</param>
        /// <returns>Identifier that was passed to create a given variable name.</returns>
        public string ResolveNameToIdentifier(string name)
        {
            if (name.Length==0)
            {
                throw new InvalidOperationException("Cannot resolve an identifier for empty variable name.");
            }

            return name.TrimEnd('0','1','2','3','4','5','6','7','8','9');
        }
        #endregion
    }
}