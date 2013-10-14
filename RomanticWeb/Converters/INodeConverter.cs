using System;
using System.Collections.Generic;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    public interface INodeConverter
    {
        IEnumerable<object> ConvertNodes(Uri predicate,IEnumerable<Node> objects);
    }
}