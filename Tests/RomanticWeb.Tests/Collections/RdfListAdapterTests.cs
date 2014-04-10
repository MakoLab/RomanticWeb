using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Collections;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Tests.Collections
{
    [TestFixture]
    public class RdfListAdapterTests
    {
        private Mock<IEntityContext> _contextMock;
        private IEntityContext _context;
        private Mock<IEntity> _entity;
        private OverridingGraphSelector _override;

        [SetUp]
        public void Setup()
        {
            _override=new OverridingGraphSelector(
                "urn:actual:entity",
                new Mock<IEntityMapping>().Object,
                new Mock<IPropertyMapping>().Object);

            var rdfNilMock=new Mock<IRdfListNode>();
            rdfNilMock.SetupAllProperties();
            rdfNilMock.Setup(m => m.Id).Returns(Vocabularies.Rdf.nil);
            _contextMock=new Mock<IEntityContext>();
            _contextMock.Setup(c => c.BlankIdGenerator).Returns(new DefaultBlankNodeIdGenerator());
            _contextMock.Setup(c => c.Load<IRdfListNode>(Vocabularies.Rdf.nil,false)).Returns((EntityId entityId,bool check) => CreateListNode(entityId));
            _contextMock.Setup(c => c.Create<IRdfListNode>(It.IsAny<BlankId>())).Returns((EntityId entityId) => CreateListNode(entityId));
            _context=_contextMock.Object;
            _entity=new Mock<IEntity>();
            _entity.SetupGet(entity => entity.Context).Returns(_contextMock.Object);
        }

        [Test]
        public void Empty_list_should_have_count_zero()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override);

            // then
            list.Count.Should().Be(0);
        }

        [Test]
        public void Should_allow_adding_elements()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override);

            // when
            list.Add(4);
            list.Add(8);

            // then
            list.Count.Should().Be(2);
            _contextMock.Verify(c => c.Create<IRdfListNode>(It.IsAny<BlankId>()),Times.Exactly(2));
        }

        [TestCase(0,new[] { 5,4,8,41,666 })]
        [TestCase(1,new[] { 4,5,8,41,666 })]
        [TestCase(4,new[] { 4,8,41,666,5 })]
        public void Should_allow_inserting_elements_at_index(int index,int[] expectedCollection)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,41,666 };

            // when
            list.Insert(index,5);

            // then
            list.Count.Should().Be(5);
            list.Should().ContainInOrder(expectedCollection);
            _contextMock.Verify(c => c.Create<IRdfListNode>(It.IsAny<BlankId>()),Times.Exactly(5));
        }

        [TestCase(0,new[] { 5,4,8,41,666 })]
        [TestCase(1,new[] { 4,5,8,41,666 })]
        [TestCase(4,new[] { 4,8,41,666,5 })]
        public void Should_allow_inserting_elements_at_index_with_indexer(int index,int[] expectedCollection)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,41,666 };

            // when
            list[index]=5;

            // then
            list.Count.Should().Be(5);
            list.Should().ContainInOrder(expectedCollection);
            _contextMock.Verify(c => c.Create<IRdfListNode>(It.IsAny<BlankId>()),Times.Exactly(5));
        }

        [Test]
        public void Should_throw_when_inserting_to_invalid_index([Values(-1,5,100)]int index)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,41,666 };

            // then
            list.Invoking(l => l.Insert(index,10))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Should_initialize_with_type_initializer_in_correct_order()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,41,666 };

            // then
            list.Should().ContainInOrder(4,8,41,666);
            _contextMock.Verify(c => c.Create<IRdfListNode>(It.IsAny<BlankId>()),Times.Exactly(4));
        }

        [Test]
        public void Should_allow_getting_elements_by_index()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8 };

            // then
            list[0].Should().Be(4);
            list[1].Should().Be(8);
            _contextMock.Verify(c => c.Create<IRdfListNode>(It.IsAny<BlankId>()),Times.Exactly(2));
        }

        [Test]
        public void Should_throw_when_getting_elements_by_invalid_index([Values(-1,4,666)]int index)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,41,666 };

            // then
            list.Invoking(l => { var i=l[index]; })
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Should_calculate_count()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,5,6,7,8 };

            // then
            list.Count.Should().Be(5);
        }

        [TestCase(4,new[] { 8,8,41,666 },true)]
        [TestCase(8,new[] { 4,8,41,666 },true)]
        [TestCase(20,new[] { 4,8,8,41,666 },false)]
        [TestCase(666,new[] { 4,8,8,41 },true)]
        public void Should_allow_removing_elements(int elementToRemove,int[] expectedResultCollection,bool expectedReturnValue)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,8,41,666 };

            // when
            var removeResult=list.Remove(elementToRemove);

            // then
            list.Should().ContainInOrder(expectedResultCollection);
            removeResult.Should().Be(expectedReturnValue);
            if (expectedReturnValue)
            {
                _contextMock.Verify(c => c.Delete(It.IsAny<BlankId>()),Times.Once);
            }
            else
            {
                _contextMock.Verify(c => c.Delete(It.IsAny<BlankId>()),Times.Never);
            }
        }

        [Test]
        public void Removing_last_element_and_adding_new_to_end_should_keep_list_linked()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,8,41,666 };

            // when
            list.Remove(666);
            list.Add(1337);

            // then
            list.Should().ContainInOrder(4,8,8,41,1337);
        }

        [Test]
        public void Should_allow_removing_last_element()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 666 };

            // when
            list.Remove(666);

            // then
            list.Should().BeEmpty();
        }

        [Test]
        public void Should_allow_removing_last_elements_and_add_new_afterwards()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 666 };

            // when
            list.Remove(666);
            list.Add(1337);
            list.Add(999);

            // then
            list.Should().ContainInOrder(1337,999);
        }

        [TestCase(0,new[] { 8,8,41,666 })]
        [TestCase(2,new[] { 4,8,41,666 })]
        [TestCase(4,new[] { 4,8,8,41 })]
        public void Should_allow_removing_elements_by_index(int index,int[] expectedResultCollection)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,8,41,666 };

            // when
            list.RemoveAt(index);

            // then
            list.Should().ContainInOrder(expectedResultCollection);
            _contextMock.Verify(c => c.Delete(It.IsAny<BlankId>()),Times.Exactly(1));
        }

        [TestCase(4,true)]
        [TestCase(41,true)]
        [TestCase(1337,false)]
        public void Should_allow_checking_item_existence(int item,bool expectedResult)
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,8,41,666 };

            // when
            var contains=list.Contains(item);

            // then
            contains.Should().Be(expectedResult);
        }

        [Test]
        public void Should_allow_clearing_entire_list()
        {
            // given
            var list=new RdfListAdapter<int>(_context,_entity.Object,_override) { 4,8,8,41,666 };

            // when
            list.Clear();

            // then
            list.Should().BeEmpty();
            _contextMock.Verify(c => c.Delete(It.IsAny<BlankId>()),Times.Exactly(5));
        }

        private IRdfListNode CreateListNode(EntityId entityId)
        {
            var listNodeMock=new Mock<IRdfListNode>();
            listNodeMock.SetupAllProperties();
            listNodeMock.Setup(m => m.Id).Returns(entityId);
            return listNodeMock.Object;
        }
    }
}