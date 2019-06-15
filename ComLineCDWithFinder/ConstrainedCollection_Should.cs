using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Execution;
using FluentAssertions;
using NUnit.Framework;

namespace ComLineCDWithFinder
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class ConstrainedCollection_Should
    {
        private const string InvalidInput = "invalid";
        private const string ValidInput = "valid";

        private IConstrainedCollection<string> collection;

        private Predicate<string> Constraint => s => s != InvalidInput;

        string[] GetValidItems(int count = 5) => Enumerable.Range(1, count).Select(i => i + ValidInput).ToArray();

        void AddItemsToCollection(IConstrainedCollection<string> col, string[] items)
        {
            foreach (var str in items)
            {
                col.TryAdd(str);
            }
        }

        [SetUp]
        public void SetUp()
        {
            collection = new ConstrainedList<string>(Constraint, true, 50);
        }

        [Test]
        public void Throw_OnAdd_NullArg()
        {
            Assert.Throws<ArgumentNullException>(() => collection.Add(null));
        }

        [Test]
        public void Throw_OnTryAdd_NullArg()
        {
            Assert.Throws<ArgumentNullException>(() => collection.TryAdd(null));
        }

        [Test]
        public void Throw_OnAdd_InvalidArg()
        {
            Assert.Throws<ArgumentException>(() => collection.Add(InvalidInput));
        }

        [Test]
        public void ReturnFalse_OnTryAdd_InvalidArg()
        {
            collection.TryAdd(InvalidInput).Should().BeFalse();
        }

        [Test]
        public void NotAdd_OnAdd_InvalidArg()
        {
            try
            {
                collection.Add(InvalidInput);
            }
            catch
            {
                // ignored
            }
            collection.Should().BeEmpty();
        }

        [Test]
        public void NotAdd_OnTryAdd_InvalidArg()
        {
            collection.TryAdd(InvalidInput);
            collection.Should().BeEmpty();

        }

        [Test]
        public void ReturnTrue_OnTryAdd_ValidArg()
        {
            collection.TryAdd(ValidInput).Should().BeTrue();
        }

        [Test]
        public void AddItem_OnAdd_ValidArg()
        {
            collection.Add(ValidInput);
            collection.Should().BeEquivalentTo(ValidInput);
        }

        [Test]
        public void AddItem_OnTryAdd_ValidArg()
        {
            collection.TryAdd(ValidInput);
            collection.Should().BeEquivalentTo(ValidInput);   
        }

        [Test]
        public void AddAllItems_OnAdd_ValidItems()
        {
            var array = GetValidItems();
            foreach (var s in array)
            {
                collection.Add(s);
            }
            collection.Should().BeEquivalentTo(array);
        }

        [Test]
        public void AddAllItems_OnTryAdd_ValidItems()
        {
            var array = GetValidItems();
            foreach (var s in array)
            {
                collection.TryAdd(s);
            }
            collection.Should().BeEquivalentTo(array);
        }

        [Test]
        public void ReturnCount_OnCount()
        {
            collection.Add(ValidInput);
            collection.Add(ValidInput);
            collection.Count.Should().Be(2);
        }

        [Test]
        public void ReturnTrue_OnContains_IfElementContains()
        {
            collection.Add(ValidInput);
            AddItemsToCollection(collection,GetValidItems());
            collection.Contains(ValidInput).Should().BeTrue();
        }

        [Test]
        public void ReturnFalse_OnContains_IfElementNotContains()
        {
            collection.Add(ValidInput);
            AddItemsToCollection(collection,GetValidItems());
            collection.Contains(InvalidInput).Should().BeFalse();
        }

        [Test]
        public void RemoveItem_OnRemove()
        {
            collection.Add(ValidInput);
            var items = GetValidItems();
            AddItemsToCollection(collection, items);
            collection.Remove(ValidInput);
            collection.Should().BeEquivalentTo(items);
        }

        [Test]
        public void BeEmpty_OnClear()
        {
            AddItemsToCollection(collection, GetValidItems());
            collection.Clear();
            collection.Should().BeEmpty();
        }

        [Test]
        public void Throw_OnAdd_IfOverLimit()
        {
            AddItemsToCollection(collection, GetValidItems(50));
            Assert.Throws<ArgumentException>(() => collection.Add(ValidInput));
        }

        [Test]
        public void ReturnFalse_OnTryAdd_IfOverLimit()
        {
            AddItemsToCollection(collection, GetValidItems(50));
            collection.TryAdd(ValidInput).Should().BeFalse();
        }

        [Test]
        public void NotAdd_OnAdd_IfOverLimit()
        {
            var items = GetValidItems(50);
            AddItemsToCollection(collection, items);
            try
            {
                collection.Add(ValidInput);
            }
            catch 
            {
            }

            collection.Should().BeEquivalentTo(items);
        }

        [Test]
        public void NotAdd_OnTryAdd_IfOverLimit()
        {
            var items = GetValidItems(50);
            AddItemsToCollection(collection, items);
            collection.TryAdd(ValidInput);
            collection.Should().BeEquivalentTo(items);
        }
    }
}
