using System;

namespace RomanticWeb
{
    public interface ITripleSourceFactory
    {
        ITripleSource CreateSourceForGraph(Uri namedGraph);

        ITripleSource CreateSourceForUnionGraph();
    }
}