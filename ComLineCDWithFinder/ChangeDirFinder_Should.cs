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
        private ChangeDirFinder _cdFinder;
        private DirectoryInfo _curDirectory;
        private List<DirectoryInfo> SubDirectories { get; } = new List<DirectoryInfo>();
        private static Random _random;

        [OneTimeSetUp]
        public void StartSetUp()
        {
            _random = new Random();
            _curDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            SubDirectories.AddRange(CreateSubDirs(_curDirectory, 2,2));
        }

        [SetUp]
        public void SetUp()
        {
            _cdFinder = new ChangeDirFinder(_curDirectory.FullName);
        }

        private static IEnumerable<DirectoryInfo> CreateSubDirs(
            DirectoryInfo parent,
            int count, 
            int nestingLvl)
        {
            var dirs = new List<DirectoryInfo>();
            if (nestingLvl <= 0) return dirs.ToArray();
            var i = count;
            while (i-->0)
                dirs.Add(parent.CreateSubdirectory(GetNameForSubDir(parent)));
            var nest = new List<DirectoryInfo>();
            foreach (var dir in dirs)
            {
                nest.AddRange(CreateSubDirs(dir, count, nestingLvl - 1));
            }

            return dirs.Concat(nest).ToArray();
        }

        private static string GetNameForSubDir(DirectoryInfo parent)
        {
            var strB = new StringBuilder("dir" + _random.Next());
            while (File.Exists(parent.FullName + strB))
            {
                strB.Append(_random.Next());
            }
            return strB.ToString();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            foreach (var dir in SubDirectories)
            {
                if(dir.Exists) dir.Delete(true);
            }
            SubDirectories.Clear();
        }

        [TestCase("")]
        public void Throw_OnEmptyInput(string path)
        {
            Assert.Throws<ArgumentException>(()=>_cdFinder.GetPathTo(path));
        }

        [Test]
        public void ReturnCurDir_OnCurDirInput()
        {
            _cdFinder.GetPathTo(_curDirectory.Name).Should().Be(_curDirectory.FullName);
        }

        [Test]
        public void ReturnPath_OnSubDir()
        {
            _cdFinder.GetPathTo(SubDirectories[0].Name).Should().Be(SubDirectories[0].FullName);
        }

        [Test]
        public void ReturnPath_OnDirInSubDir()
        {
            var target = SubDirectories[1].GetDirectories().First();
            _cdFinder.GetPathTo(target.Name).Should().Be(target.FullName);
        }

        [Test]
        public void ReturnNull_IfDirNotExists()
        {
            _cdFinder.GetPathTo("iDontExist").Should().BeNullOrEmpty();
        }

        [TestCase(@"C:\", ExpectedResult = @"C:\")]
        [TestCase(@"D:\Movies", ExpectedResult = @"D:\Movies")]
        [TestCase(@"C:\Users\ASUS\Desktop\Prog",
                  ExpectedResult = @"C:\Users\ASUS\Desktop\Prog")]
        public string ReturnPath_OnFullPath(string path)
        {
            return _cdFinder.GetPathTo(path);
        }
    }
}
