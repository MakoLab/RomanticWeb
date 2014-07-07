using System;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>
    /// An implementation of <see cref="ISourceGraphSelectionOverride"/>, 
    /// which will cause the default graph to be used
    /// </summary>
    public class UnionGraphSelector : ISourceGraphSelectionOverride
    {
        /// <inheritdoc />
        public virtual Func<INamedGraphSelector, Uri> SelectGraph
        {
            get
            {
                return SelectGraphMethod;
            }
        }

        private static Uri SelectGraphMethod(INamedGraphSelector arg)
        {
            return null;
        }
    }
}