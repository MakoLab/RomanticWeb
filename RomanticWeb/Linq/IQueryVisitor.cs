using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Linq.Model;
using RomanticWeb.Mapping;

namespace RomanticWeb.Linq
{
    /// <summary>Base interface for query visitors.</summary>
    internal interface IQueryVisitor
    {
        /// <summary>Gets an associated query.</summary>
        Query Query { get; }

        /// <summary>Gets the mappings repository.</summary>
        IMappingsRepository MappingsRepository { get; }
    }
}