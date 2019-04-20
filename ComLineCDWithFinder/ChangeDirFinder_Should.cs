using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace ComLineCDWithFinder
{
    [TestFixture]
    public class ChangeDirFinder_Should
    {
        private ChangeDirFinder cdFinder;
        private DirectoryInfo curDirectory;
        [SetUp]
        public void SetUp()
        {
            curDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            cdFinder = new ChangeDirFinder(curDirectory.FullName);
        }

        [TestCase("")]
        public void Throw_OnEmptyInput(string path)
        {
            Assert.Throws<ArgumentException>(()=>cdFinder.GetPathTo(path));
        }

        [Test]
        public void ReturnCurDir_OnCurDirInput()
        {
            cdFinder.GetPathTo(curDirectory.Name).Should().Be(curDirectory.Name);
        }

//        [TestCase(@"Kontur", 
//            ExpectedResult = @"C:\Users\ASUS\Desktop\Kontur",
//            TestName = "Path to Kontur Test")]
        [TestCase(@"xampp", 
            ExpectedResult = @"C:\xampp",
            TestName = "Path to xampp Test")]
        public string ReturnFullPathToDir_OnFolderInCurDir(string dirName)
        {
            return cdFinder.GetPathTo(dirName);
        }
    }
}
