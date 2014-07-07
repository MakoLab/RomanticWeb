using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.Linq
{
    /// <summary>Creates camelCase variable names.</summary>
    public class CamelCaseVariableNamingConvention : IVariableNamingConvention
    {
        /// <inheritdoc />
        public string GetIdentifierForName(string name)
        {
            return name.CamelCase();
        }
    }
}