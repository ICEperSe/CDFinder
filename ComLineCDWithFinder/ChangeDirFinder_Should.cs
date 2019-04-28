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
        private DirectoryInfo _curDirectory;
        private static Random _random;

        [OneTimeSetUp]
        public void StartSetUp()
        {
            _random = new Random();
            _curDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            CreateSubDirs(_curDirectory, 2,3);
        }

        private static IEnumerable<DirectoryInfo> CreateSubDirs(
            DirectoryInfo parent,
            int count, 
            int nestingLvl)
        {
            var dirs = new List<DirectoryInfo>();
            if (nestingLvl <= 0) return dirs.ToArray();
            var i = count;
            while (i-- > 0)
            {
                var name = GetNameForSubDir(parent);
                dirs.Add(parent.CreateSubdirectory(name));
            }
            var nest = new List<DirectoryInfo>();
            foreach (var dir in dirs)
            {
                nest.AddRange(CreateSubDirs(dir, count, nestingLvl - 1));
            }

            return dirs.Concat(nest);
        }


        private static IEnumerable<DirectoryInfo> CreateSubDirsWithOneName(
            DirectoryInfo parent,
            int nestingLvl,
            string name)
        {
            var dirs = new List<DirectoryInfo>();
            if (nestingLvl <= 0) return dirs;
            var subDir = parent.CreateSubdirectory(name);
            dirs.Add(subDir);
            dirs.AddRange(CreateSubDirsWithOneName(subDir, nestingLvl - 1,name));
            return dirs;
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

        [TestCase("")]
        public void Throw_OnEmptyInput(string path)
        {
            Assert.Throws<ArgumentException>(()=>PathFinder.GetPathTo(_curDirectory.FullName,path));
        }

        [Test]
        public void ReturnCurDir_OnCurDirInput()
        {
            PathFinder.GetPathTo(_curDirectory.FullName,_curDirectory.Name)[0].Should().Be(_curDirectory.FullName);
        }

        [Test]
        public void ReturnPath_OnSubDir()
        {
            var dirs = _curDirectory.GetDirectories();
            PathFinder.GetPathTo(_curDirectory.FullName,dirs[0].Name)[0].Should().Be(dirs[0].FullName);
        }

        [Test]
        public void ReturnPath_OnDirInSubDir()
        {
            var dirs = _curDirectory.GetDirectories();
            var target = dirs[1].GetDirectories().First();
            PathFinder.GetPathTo(_curDirectory.FullName,target.Name)[0].Should().Be(target.FullName);
        }

        [Test]
        public void ReturnEmpty_IfDirNotExists()
        {
            PathFinder.GetPathTo(_curDirectory.FullName,"iDontExist").Should().BeNullOrEmpty();
        }

        [TestCase(@"C:\", ExpectedResult = @"C:\")]
        [TestCase(@"D:\Movies", ExpectedResult = @"D:\Movies")]
        [TestCase(@"C:\Users\ASUS\Desktop\Prog",
                  ExpectedResult = @"C:\Users\ASUS\Desktop\Prog")]
        public string ReturnPath_OnFullPath(string path)
        {
            return PathFinder.GetPathTo(_curDirectory.FullName,path)[0];
        }

        [Test]
        public void ReturnCollection_OnDirInput_IfThereAreSeveralDirs()
        {
            var dirs = CreateSubDirsWithOneName(_curDirectory,
                4,
                "oneName");
            PathFinder.GetPathTo(_curDirectory.FullName,"oneName")
                .Should()
                .BeEquivalentTo(dirs.Select(d=>d.FullName));
        }

        [TestCase("namewith |")]
        [TestCase("namewith %")]
        [TestCase("namewith <")]
        [TestCase("namewith >")]
        [TestCase("namewith $")]
        public void Throw_OnWrongSymbols(string target)
        {
            Assert.Throws<ArgumentException>(() => PathFinder.GetPathTo(_curDirectory.FullName,target));
        }

        [Test]
        public void ReturnPath_OnPathPartInput()
        {
            var dir = _curDirectory.GetDirectories().First();
            var res = dir.GetDirectories().First();
            var name = dir.Name + "\\" + res.Name;
            PathFinder.GetPathTo(_curDirectory.FullName, name)[0].Should().Be(res.FullName);
        }

        //[Test]
        public void NotThrowStackOverflowException_OnBigNesting()
        {
            
        }
    }
}
