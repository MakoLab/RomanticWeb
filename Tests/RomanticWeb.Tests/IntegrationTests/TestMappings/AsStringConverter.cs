using System;
using RomanticWeb.Converters;
using RomanticWeb.Model;

namespace RomanticWeb.Tests.IntegrationTests.TestMappings
{
    public class AsStringConverter : INodeConverter
    {
        public object Convert(Node objectNode, IEntityContext context)
        {
            return (objectNode.IsUri ? objectNode.Uri.ToString() : objectNode.IsBlank ? objectNode.BlankNode : objectNode.Literal);
        }

        public Node ConvertBack(object value)
        {
            throw new NotImplementedException();
        }
    }
}