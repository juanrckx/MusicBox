using System.Linq;
using MusicBox.DataStructures;
using Xunit;

namespace MusicBox.Tests
{
    public class DoublyLinkedListTests
    {
        [Fact]
        public void AddLast_AddsItems_IncreasesCount()
        {
            // Arrange
            var list = new DoublyLinkedList<int>();

            // Act
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            // Assert
            Assert.Equal(3, list.Count);
        }
        [Fact]
        public void AddLast_ToEmptyList_SetsHeadAndTail()
        {
            // Arrange
            var list = new DoublyLinkedList<string>();

            // Act
            list.AddLast("Test");

            // Assert
            Assert.NotNull(list.Head);
            Assert.NotNull(list.Tail);
            Assert.Equal("Test", list.Head.Data);
            Assert.Equal("Test", list.Tail.Data);
            Assert.Null(list.Head.Previous);
            Assert.Null(list.Head.Next);
        }

        [Fact]
        public void TraverseForward_ReturnsItemsInCorrectOrder()
        {
            // Arrange
            var list = new DoublyLinkedList<int>();
            list.AddLast(10);
            list.AddLast(20);
            list.AddLast(30);

            // Act
            var result = list.TraverseForward().ToList();

            // Assert
            Assert.Equal(new[] { 10, 20, 30 }, result);
        }
        [Fact]
        public void TraverseBackward_ReturnsItemsInReverseOrder()
        {
            // Arrange
            var list = new DoublyLinkedList<int>();
            list.AddLast(10);
            list.AddLast(20);
            list.AddLast(30);

            // Act
            var result = list.TraverseBackward().ToList();

            // Assert
            Assert.Equal(new[] { 30, 20, 10 }, result);
        } 
        [Fact]
        public void MultipleAddLast_MaintainsCorrectLinks()
        {
            // Arrange
            var list = new DoublyLinkedList<int>();
            list.AddLast(10);
            list.AddLast(20);
            list.AddLast(30);

            // Act
            var first = list.Head;
            var second = first.Next;
            var third = second.Next;

            // Assert
            Assert.Equal(10, first.Data);
            Assert.Equal(20, second.Data);
            Assert.Equal(30, third.Data);
            Assert.Null(first.Previous);
            Assert.Equal(second, first.Next);
            Assert.Equal(first, second.Previous);
            Assert.Equal(third, second.Next);
            Assert.Equal(second, third.Previous);
            Assert.Null(third.Next);
        }

        [Fact]
        public void EmptyList_CountIsZero()
        {
            // Arrange
            var list = new DoublyLinkedList<int>();

            // Act & Assert
            Assert.Equal(0, list.Count);
            Assert.Null(list.Head);
            Assert.Null(list.Tail);
        }
        
        [Fact]
        public void SingleItemList_HeadAndTailAreSame()
        {
            // Arrange
            var list = new DoublyLinkedList<int>();

            // Act
            list.AddLast(42);

            // Assert
            Assert.Equal(list.Head, list.Tail);
            Assert.Equal(42, list.Head.Data);
        }
    }
}
