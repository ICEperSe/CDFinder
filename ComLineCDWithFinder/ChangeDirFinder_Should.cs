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

        [SetUp]
        public void SetUp()
        {
            _curDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            SubDirectories.AddRange(CreateSubDirs(_curDirectory, 1));

            _cdFinder = new ChangeDirFinder(_curDirectory.FullName);
        }

        private IEnumerable<DirectoryInfo> CreateSubDirs(DirectoryInfo parent,int nestingLvl)
        {
            var dirs = new List<DirectoryInfo>();
            while (nestingLvl-- > 0)
            {
                dirs.Add(parent.CreateSubdirectory(GetNameForSubDir(parent)));
            }

            return dirs.ToArray();
        }

        private static string GetNameForSubDir(DirectoryInfo parent)
        {
            var rand = new Random();
            var strB = new StringBuilder("dir" + rand.Next());
            while (File.Exists(parent.FullName + strB))
            {
                strB.Append(rand.Next());
            }
            return strB.ToString();
        }

        [TearDown]
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
    }
}
