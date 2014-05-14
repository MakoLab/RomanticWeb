using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Collections;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests.Collections
{
    [TestFixture]
    public class RdfDictionaryTests
    {
        private readonly EntityId Id=new EntityId("urn:parent:entity");
        private Mock<IEntityContext> _contextMock;
        private IEntityContext _context;
        private TestOwner _owner;

        [SetUp]
        public void Setup()
        {
            _owner=new TestOwner(Id);
            _contextMock=new Mock<IEntityContext>(MockBehavior.Strict);
            _contextMock.Setup(c => c.BlankIdGenerator).Returns(new DefaultBlankNodeIdGenerator());
            _contextMock.Setup(c => c.Create<TestPair>(It.IsAny<EntityId>())).Returns((EntityId entityId) => CreateKeyValuePair(entityId));
            _contextMock.Setup(c => c.Load<TestOwner>(Id)).Returns(_owner);
            _context=_contextMock.Object;
        }

        [Test]
        public void Empty_dictionary_should_have_count_zero()
        {
            // given
            var dict=CreateRdfDictionary();

            // then
            dict.Count.Should().Be(0);
        }

        [Test]
        public void Should_allow_adding_elements()
        {
            // given
            var dict = CreateRdfDictionary();

            // when
            dict.Add("a",5);
            dict["b"]=6;
            dict.Add(new KeyValuePair<string,int>("c",7));

            // then
            dict.Count.Should().Be(3);
            dict.Should().Contain("a",5).And.Contain("b",6).And.Contain("c",7);
        }

        [Test]
        public void Should_allow_getting_elements()
        {
            // given
            var dict = CreateRdfDictionary();
            dict.Add("a",5);

            // when
            var value=dict["a"];

            // then
            value.Should().Be(5);
        }

        [Test]
        public void Should_throw_when_getting_element_for_invalid_index()
        {
            // given
            var dict = CreateRdfDictionary();

            // then
            dict.Invoking(d => { var value = d["xyz"]; }).ShouldThrow<KeyNotFoundException>();
        }

        [Test]
        public void Should_allow_getting_keys()
        {
            // given
            var dict=CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("ę",5);

            // when
            var keys=dict.Keys;

            // then
            keys.Should().BeEquivalentTo(new[] { "a","b","c","d","ę" });
        }
        
        [Test]
        public void Should_allow_getting_values()
        {
            // given
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("ę",55);

            // when
            var values=dict.Values;

            // then
            values.Should().BeEquivalentTo(new[] { 1,2,3,4,55 });
        }

        [Test]
        public void Should_allow_removing_elements_by_key()
        {
            // given
            _contextMock.Setup(ctx => ctx.Delete(It.IsAny<EntityId>()));
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            var wasRemoved = dict.Remove("a");

            // then
            wasRemoved.Should().BeTrue();
            _contextMock.Verify(c => c.Delete(It.IsAny<EntityId>()), Times.Once());
            dict.Should().HaveCount(4);
        }

        [Test]
        [Sequential]
        public void Should_allow_removing_key_which_doesnt_exist(
            [Values("a","f")]string key, [Values(2,1)]int value)
        {
            // given
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            var wasRemoved = dict.Remove(new KeyValuePair<string,int>(key,value));

            // then
            wasRemoved.Should().BeFalse();
            _contextMock.Verify(c => c.Delete(It.IsAny<BlankId>()),Times.Never());
        }

        [Test]
        public void Should_allow_removing_KeyValuePair_which_doesnt_exist()
        {
            // given
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            var wasRemoved = dict.Remove("f");

            // then
            wasRemoved.Should().BeFalse();
            _contextMock.Verify(c => c.Delete(It.IsAny<BlankId>()), Times.Never());
        }

        [Test]
        public void Should_allow_removing_elements_by_KeyValuePair()
        {
            // given
            _contextMock.Setup(ctx => ctx.Delete(It.IsAny<EntityId>()));
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            var wasRemoved=dict.Remove(new KeyValuePair<string,int>("a",1));

            // then
            wasRemoved.Should().BeTrue();
            _contextMock.Verify(c => c.Delete(It.IsAny<EntityId>()),Times.Once());
            dict.Should().HaveCount(4);
        }

        [Test]
        public void Should_allow_enumerating()
        {
            // given
            const char StartChar=(char)('a'-1);
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            int i=1;
            foreach (KeyValuePair<string,int> keyValuePair in dict)
            {
                // then
                var currentChar=(char)(StartChar+i);
                keyValuePair.Key.Should().Be(currentChar.ToString(CultureInfo.InvariantCulture));
                keyValuePair.Value.Should().Be(i++);
            }
        }

        [Test]
        public void Should_allow_checking_for_pair_existence()
        {
            // given
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // then
            dict.Contains(new KeyValuePair<string, int>("b", 2)).Should().BeTrue();
            dict.Contains(new KeyValuePair<string, int>("f", 6)).Should().BeFalse();
        }

        [Test]
        [Sequential]
        public void Should_allow_TryGetting_existing_values(
            [Values("a","f")]string key,
            [Values(1,0)] int expectedValue,
            [Values(true,false)]bool methodResult)
        {
            // given
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            int value;
            var result=dict.TryGetValue(key,out value);

            // then
            result.Should().Be(methodResult);
            value.Should().Be(expectedValue);
        }

        [Test]
        public void Should_allow_clearing_collection()
        {
            // given
            _contextMock.Setup(ctx => ctx.Delete(It.IsAny<EntityId>()));
            var dict = CreateRdfDictionary();
            dict.Add("a",1);
            dict.Add("b",2);
            dict.Add("c",3);
            dict.Add("d",4);
            dict.Add("e",5);

            // when
            dict.Clear();

            // then
            dict.Should().BeEmpty();
            _contextMock.Verify(ctx=>ctx.Delete(It.IsAny<EntityId>()),Times.Exactly(5));
        }

        [Test]
        public void Should_allow_creating_with_existing_entities()
        {
            // given
            var entries=from i in Enumerable.Range(1,5) 
                        select new TestPair(new BlankId(string.Format("entry{0}",i)))
                                   {
                                       Key = ('a'-1+i).ToString(),
                                       Value = i
                                   };
            _owner.DictionaryEntries=entries.ToList();

            // when
            var dict=CreateRdfDictionary();

            // then
            dict.Should().HaveCount(5);
        }

        private TestPair CreateKeyValuePair(EntityId entityId)
        {
            return new TestPair(entityId);
        }

        private RdfDictionary<string,int,TestPair,TestOwner> CreateRdfDictionary()
        {
            return new RdfDictionary<string,int,TestPair,TestOwner>(Id,_context);
        }

        internal class TestPair:IDictionaryEntry<string, int>
        {
            private readonly EntityId _id;

            public TestPair(EntityId id)
            {
                _id=id;
            }

            public EntityId Id
            {
                get
                {
                    return _id;
                }
            }

            public IEntityContext Context
            {
                get
                {
                    return null;
                }
            }

            public string Key { get; set; }

            public int Value { get; set; }
        }

        internal class TestOwner:IDictionaryOwner<TestPair,string,int>
        {
            public TestOwner(EntityId id)
            {
                Id=id;
                DictionaryEntries=new List<TestPair>();
            }

            public EntityId Id { get; private set; }

            public IEntityContext Context
            {
                get
                {
                    return null;
                }
            }

            public ICollection<TestPair> DictionaryEntries { get; set; }
        }
    }
}