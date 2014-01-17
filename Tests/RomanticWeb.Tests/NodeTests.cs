using System;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void Identical_URI_nodes_should_be_equal()
        {
            // given
            var uri = new Uri("urn:test:node");

            // when
            IComparable left = Node.ForUri(uri);
            var right = Node.ForUri(uri);

            // then
            Assert.That(left.CompareTo(right), Is.EqualTo(0));
        }

        [Test]
        public void URI_node_should_be_more_than_literal_node()
        {
            // given
            var uri = new Uri("urn:test:node");

            // when
            IComparable left = Node.ForUri(uri);
            IComparable right = Node.ForLiteral("some value");

            // then
            Assert.That(left.CompareTo(right), Is.GreaterThan(0));
            Assert.That(right.CompareTo(left), Is.LessThan(0));
        }

        [Test]
        public void URI_node_should_be_more_than_blank_node()
        {
            // given
            var uri = new Uri("urn:test:node");

            // when
            IComparable left = Node.ForUri(uri);
            IComparable right = Node.ForBlank("blank",new EntityId("urn:test:node"),new Uri("urn:some:graph"));

            // then
            Assert.That(left.CompareTo(right), Is.GreaterThan(0));
            Assert.That(right.CompareTo(left), Is.LessThan(0));
        }

        [Test]
        public void Blank_node_should_be_more_than_literal_node()
        {
            // when
            IComparable left = Node.ForBlank("blank",new EntityId("urn:test:node"),null);
            IComparable right = Node.ForLiteral("some value");

            // then
            Assert.That(left.CompareTo(right), Is.GreaterThan(0));
            Assert.That(right.CompareTo(left), Is.LessThan(0));
        }

        [Test]
        public void Two_literal_with_different_values_should_order_by_value_and_ignore_datatype(
            [Values("other literal")]string otherValue,
            [Values("urn:some:datatype", null)] string datatype)
        {
            // given
            const string Value = "literal";

            // when
            IComparable left = Node.ForLiteral(Value);
            IComparable right = datatype == null
                ? Node.ForLiteral(otherValue)
                : Node.ForLiteral(otherValue, new Uri(datatype));

            // then
            Assert.That(left.CompareTo(right), Is.EqualTo(String.Compare(Value, otherValue, StringComparison.Ordinal)));
            Assert.That(right.CompareTo(left), Is.EqualTo(String.Compare(otherValue, Value, StringComparison.Ordinal)));
        }

        [Test]
        public void Two_literal_with_different_values_should_order_by_value_and_ignore_language(
            [Values("other literal")]string otherValue,
            [Values("pl", null)] string language)
        {
            // given
            const string Value = "literal";

            // when
            IComparable left = Node.ForLiteral(Value);
            IComparable right = language == null
                ? Node.ForLiteral(otherValue)
                : Node.ForLiteral(otherValue, language);

            // then
            Assert.That(left.CompareTo(right), Is.EqualTo(String.Compare(Value, otherValue, StringComparison.Ordinal)));
            Assert.That(right.CompareTo(left), Is.EqualTo(String.Compare(otherValue, Value, StringComparison.Ordinal)));
        }

        [Test]
        public void Two_literal_nodes_should_order_by_value()
        {
            // given
            const string Value = "literal";
            const string OtherValue = "other literal";

            // when
            IComparable left = Node.ForLiteral(Value);
            IComparable right = Node.ForLiteral(OtherValue);

            // then
            Assert.That(left.CompareTo(right), Is.EqualTo(String.Compare(Value, OtherValue, StringComparison.Ordinal)));
            Assert.That(right.CompareTo(left), Is.EqualTo(String.Compare(OtherValue, Value, StringComparison.Ordinal)));
        }

        [Test]
        public void Should_thrown_when_creating_from_relative_Uri()
        {
            // given
            var relative=new Uri("/some/path", UriKind.Relative);

            // when
            var exception=Assert.Throws<ArgumentOutOfRangeException>(() => Node.ForUri(relative));

            // then
            Assert.That(exception.ParamName, Is.EqualTo("uri"));
        }
    }
}