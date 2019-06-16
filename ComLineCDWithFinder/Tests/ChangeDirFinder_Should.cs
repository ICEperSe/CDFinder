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
            int nestingLvl,
            string namePrefix = "dir")
        {
            var dirs = new List<DirectoryInfo>();
            if (nestingLvl <= 0) return dirs.ToArray();
            var i = count;
            while (i-- > 0)
            {
                var name = GetNameForSubDir(parent, namePrefix);
                dirs.Add(parent.CreateSubdirectory(name));
            }
            var nest = new List<DirectoryInfo>();
            foreach (var dir in dirs)
            {
                nest.AddRange(CreateSubDirs(dir, count, nestingLvl - 1));
            }

            return dirs.Concat(nest);
        }


        private static IEnumerable<DirectoryInfo> CreateNestingDirsWithOneName(
            DirectoryInfo parent,
            int nestingLvl,
            string name)
        {
            var dirs = new List<DirectoryInfo>();
            if (nestingLvl <= 0) return dirs;
            var subDir = parent.CreateSubdirectory(name);
            dirs.Add(subDir);
            dirs.AddRange(CreateNestingDirsWithOneName(subDir, nestingLvl - 1,name));
            return dirs;
        }

        private static IEnumerable<DirectoryInfo> GetDirs(DirectoryInfo parent, Predicate<DirectoryInfo> rule)
        {
            var res = new List<DirectoryInfo>();
            res.AddRange(parent.GetDirectories().Where(d=>rule(d)));
            foreach (var dir in parent.GetDirectories())
            {
                res.AddRange(GetDirs(dir, rule));
            }

            return res;
        }

        private static string GetNameForSubDir(DirectoryInfo parent, string namePrefix)
        {
            var strB = new StringBuilder(namePrefix + _random.Next());
            while (File.Exists(parent.FullName + strB))
            {
                strB.Append(_random.Next());
            }
            return strB.ToString();
        }

        [Test]
        public void Throw_OnEmptyInput()
        {
            Assert.Throws<ArgumentException>(()=>
                PathFinder.GetPath(_curDirectory.FullName,string.Empty)
                );
        }

        [Test]
        public void ReturnCurDir_OnCurDir()
        {
            PathFinder.GetPath(_curDirectory.FullName,_curDirectory.Name)[0].Should().Be(_curDirectory.FullName);
        }

        [Test]
        public void ReturnPath_OnSubDir()
        {
            var dirs = _curDirectory.GetDirectories();
            PathFinder.GetPath(_curDirectory.FullName,dirs[0].Name)[0].Should().Be(dirs[0].FullName);
        }

        [Test]
        public void ReturnPath_OnDirInSubDir()
        {
            var dirs = _curDirectory.GetDirectories();
            var target = dirs[1].GetDirectories().First();
            PathFinder.GetPath(_curDirectory.FullName,target.Name)[0]
                .Should()
                .Be(target.FullName);
        }

        [Test]
        public void ReturnEmpty_IfDirNotExists()
        {
            PathFinder.GetPath(_curDirectory.FullName,"iDontExist").Should().BeNullOrEmpty();
        }

        [TestCase(@"C:\", ExpectedResult = @"C:\")]
        [TestCase(@"D:\Movies", ExpectedResult = @"D:\Movies")]
        [TestCase(@"C:\Users\ASUS\Desktop\Prog",
                  ExpectedResult = @"C:\Users\ASUS\Desktop\Prog")]
        public string ReturnPath_OnFullPath(string path)
        {
            return PathFinder.GetPath(_curDirectory.FullName,path)[0];
        }

        [Test]
        public void ReturnCollection_OnDirInput_IfThereAreSeveralDirs()
        {
            var dirs = CreateNestingDirsWithOneName(_curDirectory,
                4,
                "oneName");
            PathFinder.GetPath(_curDirectory.FullName,"oneName")
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
            Assert.Throws<ArgumentException>(() => PathFinder.GetPath(_curDirectory.FullName,target));
        }

        [TestCase('\\')]
        [TestCase('/')]
        public void ReturnPath_OnPathPart(char separator)
        {
            var dir = _curDirectory.GetDirectories().First();
            var res = dir.GetDirectories().First();
            var name = dir.Name + separator + res.Name;
            PathFinder.GetPath(_curDirectory.FullName, name)[0].Should().Be(res.FullName);
        }

        [Test]
        public void ReturnEmpty_OnPartOfRealDirName()
        {
            PathFinder.GetPath(_curDirectory.FullName, "dir")
                .Should()
                .BeNullOrEmpty();
        }

        [Test]
        public void ReturnPath_OnAsterisk_InName()
        {
            var parent = _curDirectory.CreateSubdirectory(GetNameForSubDir(_curDirectory,""));
            var dirs = CreateSubDirs(parent, 2, 3).ToArray();
            PathFinder.GetPath(parent.FullName, "dir*")
                .Should()
                .BeEquivalentTo(dirs.Select(d => d.FullName));
        }

        [TestCase('\\')]
        [TestCase('/')]
        public void ReturnPath_OnAsterisk_InPathPart(char separator)
        {
            var target = _curDirectory
                .GetDirectories()
                .First(d => d.Name.StartsWith("dir"))
                .GetDirectories()
                .First();
            var path = "dir*" + separator + target.Name;
            PathFinder.GetPath(_curDirectory.FullName, path)
                .Should()
                .BeEquivalentTo(target.FullName);
        }

        [TestCase('\\')]
        [TestCase('/')]
        public void ReturnPath_OnAsterisk_InPathPartAndName(char separator)
        {
            var dirs = GetDirs(_curDirectory, d=>d.GetDirectories().Length == 0).Where(d=>d.Name.StartsWith("dir"));
            var path = "dir*" + separator + "dir*" + separator + "dir*";
            PathFinder.GetPath(_curDirectory.FullName, path)
                .Should()
                .BeEquivalentTo(dirs.Select(d=>d.FullName));
        }

        [Test]
        public void ReturnPath_OnAsterisk_AfterDriveName()
        {
            var pathStrings = _curDirectory
                .FullName
                .Split(Path.DirectorySeparatorChar);
            var inputPath = new StringBuilder(pathStrings[0]);
            for (var i = 1; i < pathStrings.Length-1; i++)
                inputPath.Append(Path.DirectorySeparatorChar + PathFinder.Asterisk);
            inputPath.Append(Path.DirectorySeparatorChar +_curDirectory.Name);
            PathFinder.GetPath(_curDirectory.FullName, inputPath.ToString())
                .Should()
                .BeEquivalentTo(_curDirectory.FullName);
        }

        [Test]
        public void ReturnSubDirsList_OnAsterisk_Only()
        {
            PathFinder.GetPath(_curDirectory.FullName, "*")
                .Should()
                .BeEquivalentTo(GetDirs(_curDirectory, d=>true)
                    .Select(d => d.FullName));
        }

        //[Test]
        //todo: public void NotThrowStackOverflowException_OnBigNesting()
        //{
            
        //}
    }
}
