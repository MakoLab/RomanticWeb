using System;

namespace RomanticWeb.Linq
{
    /// <summary>Provides a base interface for variable naming strategies.</summary>
    internal interface IVariableNamingStrategy
    {
        /// <summary>Gets a variable name for given identifier.</summary>
        /// <param name="identifier">Base identifier for which the name must be created.</param>
        /// <returns>Name of the variale coresponding for given identifier.</returns>
        string GetNameForIdentifier(string identifier);

        /// <summary>Reverses the process and resolves an initial identifier passed to create a variable name.</summary>
        /// <param name="name">Name to be resolved.</param>
        /// <returns>Identifier that was passed to create a given variable name.</returns>
        string ResolveNameToIdentifier(string name);
    }
}