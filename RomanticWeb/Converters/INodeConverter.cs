using System.Collections.Generic;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    public interface INodeConverter
    {
        object Convert(Node objectNode,IEntityContext context);

        Node ConvertBack(object value);
    }
}