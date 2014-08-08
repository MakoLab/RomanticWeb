using RomanticWeb.Entities;
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

        /// <summary>Gets the base Uri selection policy.</summary>
        IBaseUriSelectionPolicy BaseUriSelector { get; }
    }
}