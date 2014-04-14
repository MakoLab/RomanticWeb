using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.Linq
{
    /// <summary>Represents a base interface for variable naming conventions.</summary>
    public interface IVariableNamingConvention
    {
        /// <summary>Gets the identifier name in given convention for given name.</summary>
        /// <param name="name">Name to be transformed.</param>
        /// <returns><see cref="System.String" /> representing an identifier build from given name.</returns>
        string GetIdentifierForName(string name);
    }
}