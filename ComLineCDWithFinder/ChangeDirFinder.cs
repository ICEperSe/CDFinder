using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    public class ChangeDirFinder
    {
        public DirectoryInfo CurrentDirectory;

        private static readonly char[] InvalidSymbols = new[] {'%', '|', '<', '>', '$'};

        public ChangeDirFinder(string curDir)
        {
            CurrentDirectory = new DirectoryInfo(curDir);
        }

        public string[] GetPathTo(string targetDir)
        {
            if(targetDir == string.Empty)
                throw new ArgumentException(nameof(targetDir));
            if(targetDir == CurrentDirectory.Name)
                return new []{CurrentDirectory.FullName};
            if (Path.IsPathRooted(targetDir) && Directory.Exists(targetDir)) 
                return new []{targetDir};
            return Find(CurrentDirectory, targetDir).Select(d=>d.FullName).ToArray();
        }

        private IEnumerable<DirectoryInfo> Find(DirectoryInfo parent, string dirName)
        {
            var subDirs = parent.GetDirectories();
            var dirs = new List<DirectoryInfo>();
            var target = subDirs.FirstOrDefault(d => d.Name == dirName);
            if (target != null) dirs.Add(target);
            foreach (var subDir in subDirs)
            {
                dirs.AddRange(Find(subDir, dirName));
            }
            return dirs;
        }
    }
}
