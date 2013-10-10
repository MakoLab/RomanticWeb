using System;
using System.Collections.Generic;

namespace RomanticWeb
{
    public interface INodeProcessor
    {
        IEnumerable<object> ProcessNodes(Uri predicate,IEnumerable<Node> objects);
    }
}