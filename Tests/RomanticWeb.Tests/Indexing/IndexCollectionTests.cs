using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Model;

namespace RomanticWeb.Tests.Indexing
{
    [TestFixture]
    public class IndexCollectionTests
    {
        private const string Subject1="http://test.org/subject1";
        private const string Subject2="http://test.org/subject2";
        private const string Subject3="http://test.org/subject3";
        private IndexCollection<string> _subjects;

        [SetUp]
        protected void Setup()
        {
            _subjects=new IndexCollection<string>();
            _subjects.Add(Subject1,0,2);
            _subjects.Add(Subject2,2,2);
            _subjects.Add(Subject3,4,2);
        }

        [Test]
        [TestCase(Subject1,IndexCollection<string>.FirstPossible,0,2)]
        [TestCase(Subject1,1,0,2)]
        [TestCase(Subject1,2,-1,-1)]
        [TestCase(Subject2,IndexCollection<string>.FirstPossible,2,2)]
        [TestCase(Subject2,2,2,2)]
        [TestCase(Subject2,4,-1,-1)]
        [TestCase(Subject3,IndexCollection<string>.FirstPossible,4,2)]
        [TestCase(Subject3,4,4,2)]
        [TestCase(Subject3,6,-1,-1)]
        public void Should_retrieve_an_index(string subject,int startAt,int expectedStartAt,int expectedLength)
        {
            // When
            Index<string> index=_subjects[subject,startAt];

            // Then
            if (expectedStartAt!=-1)
            {
                index.Should().NotBeNull();
                index.StartAt.Should().Be(expectedStartAt);
                index.Length.Should().Be(expectedLength);
            }
            else
            {
                index.Should().BeNull();
            }
        }

        [Test]
        [TestCase(Subject1,0,3,0,3,5)]
        [TestCase(Subject2,2,4,0,2,6)]
        [TestCase(Subject3,4,3,0,2,4)]
        [TestCase(Subject1,0,1,0,1,3)]
        [TestCase(Subject2,2,1,0,2,3)]
        [TestCase(Subject3,4,1,0,2,4)]
        public void Should_update_existing_entries(string subject,int startAt,int length,int expectedStartAt1,int expectedStartAt2,int expectedStartAt3)
        {
            // When
            _subjects.Set(subject,startAt,length);

            // Then
            _subjects[Subject1,IndexCollection<string>.FirstPossible].StartAt.Should().Be(expectedStartAt1);
            _subjects[Subject2,IndexCollection<string>.FirstPossible].StartAt.Should().Be(expectedStartAt2);
            _subjects[Subject3,IndexCollection<string>.FirstPossible].StartAt.Should().Be(expectedStartAt3);
        }

        [Test]
        [TestCase(Subject1,0,0,-1,0,2)]
        [TestCase(Subject2,2,0,0,-1,2)]
        [TestCase(Subject3,4,0,0,2,-1)]
        public void Should_remove_empty_index(string subject,int startAt,int length,int expectedStartAt1,int expectedStartAt2,int expectedStartAt3)
        {
            // When
            _subjects.Set(subject,startAt,length);

            // Then
            if (expectedStartAt1==-1)
            {
                _subjects[Subject1,IndexCollection<string>.FirstPossible].Should().BeNull();
            }
            else
            {
                _subjects[Subject1,IndexCollection<string>.FirstPossible].StartAt.Should().Be(expectedStartAt1);
            }

            if (expectedStartAt2==-1)
            {
                _subjects[Subject2,IndexCollection<string>.FirstPossible].Should().BeNull();
            }
            else
            {
                _subjects[Subject2,IndexCollection<string>.FirstPossible].StartAt.Should().Be(expectedStartAt2);
            }

            if (expectedStartAt3==-1)
            {
                _subjects[Subject3,IndexCollection<string>.FirstPossible].Should().BeNull();
            }
            else
            {
                _subjects[Subject3,IndexCollection<string>.FirstPossible].StartAt.Should().Be(expectedStartAt3);
            }
        }

        [Test]
        [TestCase(Subject1,0)]
        [TestCase(Subject2,1)]
        [TestCase(Subject3,2)]
        public void Should_remove_index_when_removing_single_item(string subject,int indexOf)
        {
            // Given
            _subjects[Subject1,IndexCollection<string>.FirstPossible].Length=1;
            _subjects[Subject2,IndexCollection<string>.FirstPossible].Length=1;
            _subjects[Subject3,IndexCollection<string>.FirstPossible].Length=1;
            _subjects[Subject2,IndexCollection<string>.FirstPossible].StartAt=1;
            _subjects[Subject3,IndexCollection<string>.FirstPossible].StartAt=2;

            // When
            _subjects.Remove(subject,indexOf);

            // Then
            _subjects[subject,IndexCollection<string>.FirstPossible].Should().BeNull();
        }
    }
}