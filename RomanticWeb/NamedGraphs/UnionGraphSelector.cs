using System;

namespace RomanticWeb.NamedGraphs
{
    public class UnionGraphSelector:ISourceGraphSelectionOverride
    {
        public virtual Func<INamedGraphSelector,Uri> SelectGraph
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