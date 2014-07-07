using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Dynamic;

namespace RomanticWeb.Tests.Dynamic
{
    [TestFixture]
    public class DictionaryEntityNamesTests
    {
        private DictionaryEntityNames _names;

        [SetUp]
        public void Setup()
        {
            _names = new DictionaryEntityNamesTestable("RomanticWeb.Tests", "ITestEntity", "SomeProperty", "The.Assembly");
        }

        [Test]
        public void Should_construct_correct_Entry_type_name()
        {
            _names.EntryTypeName.Should().Be("ITestEntity_SomeProperty_Entry");
        }

        [Test]
        public void Should_construct_correct_Owner_type_name()
        {
            _names.OwnerTypeName.Should().Be("ITestEntity_SomeProperty_Owner");
        }

        [Test]
        public void Should_construct_correct_Entry_fully_qualified_name()
        {
            _names.EntryTypeFullyQualifiedName.Should().Be("RomanticWeb.Tests.ITestEntity_SomeProperty_Entry, The.Assembly");
        }

        [Test]
        public void Should_construct_correct_Owner_fully_qualified_name()
        {
            _names.OwnerTypeFullyQualifiedName.Should().Be("RomanticWeb.Tests.ITestEntity_SomeProperty_Owner, The.Assembly");
        }

        [Test]
        public void Should_retain_namespace()
        {
            _names.Namespace.Should().Be("RomanticWeb.Tests");
        }

        private class DictionaryEntityNamesTestable : DictionaryEntityNames
        {
            public DictionaryEntityNamesTestable(string @namespace, string entityTypeName, string propertyName, string assemblyName)
                : base(@namespace, entityTypeName, propertyName, assemblyName)
            {
            }
        }
    }
}