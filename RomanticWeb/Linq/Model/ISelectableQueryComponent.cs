using System.Collections.Generic;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides a base interface for components that can be selected.</summary>
    public interface ISelectableQueryComponent:IQueryComponent
    {
        /// <summary>Gets an enumeration of selectable expressions.</summary>
        IEnumerable<IExpression> Expressions { get; }
    }
}