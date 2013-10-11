using System;
using System.Collections.Generic;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    public interface INodeProcessor
    {
        IEnumerable<object> ProcessNodes(Uri predicate,IEnumerable<Node> objects);
    }
}