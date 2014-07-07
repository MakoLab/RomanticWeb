using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Model;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class DictionaryTestsBase : IntegrationTestsBase
    {
        [Test]
        public void Should_allow_getting_dictionary_with_default_key_value_mapped_using_attributes_to_QName_property()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/HtmlText");

            // when
            var dict = entity.SettingsDefault;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain("mode", 1).And.Contain("source", "some text");
        }

        [Test]
        public void Should_allow_getting_dictionary_with_default_key_value_mapped_using_attributes_to_Uri_property()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/HtmlText");

            // when
            var dict = entity.StringIntDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain("padding", 20).And.Contain("margin", 30);
        }

        [Test]
        public void Should_allow_getting_dictionary_with_custom_key_mapped_using_attributes_to_QName()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomKey");

            // when
            var dict = entity.CustomQNameKeyDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain("fatherName", "Albert").And.Contain("motherName", "Eva");
        }

        [Test]
        public void Should_allow_getting_dictionary_with_custom_key_mapped_using_attributes_to_Uri_string()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomKey");

            // when
            var dict = entity.CustomUriKeyDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain("fatherName", "Albert").And.Contain("motherName", "Eva");
        }

        [Test]
        public void Should_allow_getting_dictionary_with_custom_value_mapped_using_attributes_to_QName()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomValue");

            // when
            var dict = entity.CustomQNameValueDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain("age", 28).And.Contain("height", 182);
        }

        [Test]
        public void Should_allow_getting_dictionary_with_custom_value_mapped_using_attributes_to_Uri_string()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomValue");

            // when
            var dict = entity.CustomUriValueDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain("age", 28).And.Contain("height", 182);
        }

        [Test]
        public void Should_allow_getting_dictionary_with_custom_key_and_value_mapped_using_attributes_to_QName()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomKeyValue");

            // when
            var dict = entity.CustomKeyValueQNameDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain(10, 15).And.Contain(20, 31);
        }

        [Test]
        public void Should_allow_getting_dictionary_with_custom_key_and_value_mapped_using_attributes_to_Uri_string()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomKeyValue");

            // when
            var dict = entity.CustomKeyValueUriDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain(10, 15).And.Contain(20, 31);
        }

        [Test]
        public void Should_allow_setting_a_new_dictionary()
        {
            // given
            LoadTestFile("Dictionary.trig");
            var entity = EntityContext.Load<IEntityWithDictionary>("http://magi/element/CustomKeyValue");

            // when
            entity.CustomKeyValueUriDictionary = new Dictionary<int, int> { { 5, 10 }, { 15, 25 } };
            var dict = entity.CustomKeyValueUriDictionary;

            // then
            dict.Should().HaveCount(2);
            dict.Should().Contain(5, 10).And.Contain(15, 25);
            var quads = EntityStore.Quads.Where(q => q.Graph == Node.ForUri(new Uri("http://data.magi/element/CustomKeyValue")));
            quads.Should().HaveCount(6, "Actual triples were: {0}", SerializeStore());
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }

        private string SerializeStore()
        {
            return String.Join(Environment.NewLine, EntityStore.Quads);
        }
    }
}