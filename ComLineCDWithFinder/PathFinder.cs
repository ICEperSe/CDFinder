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
    internal static class PathFinder
    {

        private static readonly char[] InvalidSymbols = new[] {'%', '|', '<', '>', '$'};

        public static string[] GetPathTo(string curDir, string targetDir)
        {
            var currentDirectory = new DirectoryInfo(curDir);
            if(targetDir == string.Empty || targetDir.IndexOfAny(InvalidSymbols) != -1)
                throw new ArgumentException(nameof(targetDir));
            if(targetDir == currentDirectory.Name)
                return new []{currentDirectory.FullName};
            if (Path.IsPathRooted(targetDir) && Directory.Exists(targetDir)) 
                return new []{targetDir};
            return Find(currentDirectory, targetDir).Select(d=>d.FullName).ToArray();
        }

        private static IEnumerable<DirectoryInfo> Find(DirectoryInfo parent, string dirName)
        {
            var subDirs = parent.GetDirectories();
            var dirs = new List<DirectoryInfo>();
            var target = subDirs.FirstOrDefault(d => d.FullName.EndsWith(dirName));
            if (target != null) dirs.Add(target);
            foreach (var subDir in subDirs)
            {
                dirs.AddRange(Find(subDir, dirName));
            }
            return dirs;
        }
    }
}
