using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Tests.Helpers
{
    internal class Nodes
    {
        private readonly int _count;

        private Nodes(int count)
        {
            _count = count;
        }

        public static Nodes Create(int count)
        {
            return new Nodes(count);
        }

        public UriNodesBuilder Uris()
        {
            return new UriNodesBuilder(_count);
        }

        public BlankNodesBuilder Blanks()
        {
            return new BlankNodesBuilder(_count);
        }

        public LiteralNodesBuilder Literals()
        {
            return new LiteralNodesBuilder(_count);
        }

        internal class UriNodesBuilder
        {
            private readonly int _count;

            private Func<int, Uri> _createUri;

            public UriNodesBuilder(int count)
            {
                _count = count;
                _createUri = i => new Uri(new Uri("http://magi/test/"), i.ToString());
            }

            public IEnumerable<Node> GetNodes()
            {
                return from i in Enumerable.Range(0, _count)
                       select Node.ForUri(_createUri(i));
            }
        }

        internal class BlankNodesBuilder
        {
            private readonly int _count;

            private Func<int, string> _createNodeIdentifier;

            private Func<int, Uri> _createGraphUri;

            public BlankNodesBuilder(int count)
            {
                _count = count;
                _createNodeIdentifier = i => string.Format("test{0}", i);
                _createGraphUri = i => null;
            }

            public IEnumerable<Node> GetNodes(EntityId entityId)
            {
                return from i in Enumerable.Range(0, _count)
                       let blankId = _createNodeIdentifier(i)
                       select Node.ForBlank(blankId, entityId, _createGraphUri(i));
            }
        }

        internal class LiteralNodesBuilder
        {
            private readonly int _count;

            private Func<int, string> _createNodeValue;
            private Func<Uri> _createDatatype;

            public LiteralNodesBuilder(int count)
            {
                _count = count;
                _createNodeValue = i => string.Format("test{0}", i);
            }

            public IEnumerable<Node> GetNodes()
            {
                var values = from i in Enumerable.Range(0, _count)
                             select _createNodeValue(i);

                if (_createDatatype != null)
                {
                    return from value in values
                           select Node.ForLiteral(value, _createDatatype());
                }

                return from value in values
                       select Node.ForLiteral(value);
            }

            public LiteralNodesBuilder WithValues(Func<int, string> createNodeValueFunc)
            {
                _createNodeValue = createNodeValueFunc;
                return this;
            }

            public LiteralNodesBuilder WithDatatype(Uri datatype)
            {
                _createDatatype = () => datatype;
                return this;
            }

            public LiteralNodesBuilder WithValues(string value)
            {
                return WithValues(i => value);
            }
        }
    }
}