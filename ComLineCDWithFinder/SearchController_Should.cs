using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace ComLineCDWithFinder
{
    [TestFixture]
    public class SearchController_Should
    {
        private readonly DirectoryInfo InvalidDir = new DirectoryInfo("invalid");
        private ISearchController<DirectoryInfo> _controller;
        private readonly Predicate<DirectoryInfo> Condition = d => d.Name != "invalid";


        [SetUp]
        public void SetUp()
        {
            _controller = new DirSearchController(Condition);
        }


        [Test]
        public void GetItem_IfSatisfy()
        {
            var valid = new DirectoryInfo("val");
            _controller.GetItem(valid);
            _controller.FoundedItems.Should().BeEquivalentTo(valid);
        }

        [Test]
        public void DontGetItem_IfNotSatisfy()
        {
            _controller.GetItem(InvalidDir);
            _controller.FoundedItems.Should().BeEmpty();
        }
    }
}
