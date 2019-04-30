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
        private static readonly char[] Separators = new[] {'\\', '/'};

        public static string[] GetPathTo(string curDir, string targetDir)
        {
            targetDir = ValidateSeparators(targetDir);
            var currentDirectory = new DirectoryInfo(curDir);
            if(targetDir == string.Empty || targetDir.IndexOfAny(InvalidSymbols) != -1)
                throw new ArgumentException(nameof(targetDir));
            if(targetDir == currentDirectory.Name)
                return new []{currentDirectory.FullName};
            if (Path.IsPathRooted(targetDir) && Directory.Exists(targetDir)) 
                return new []{targetDir};
            return Find(currentDirectory, targetDir).Select(d=>d.FullName).ToArray();
        }

        private static IEnumerable<DirectoryInfo> Find(DirectoryInfo parent, string target)
        {
            var subDirs = parent.GetDirectories();
            var dirs = new List<DirectoryInfo>();
            var name = GetDirectoryName(target);
            var targetName = subDirs.FirstOrDefault(
                d => d.FullName.EndsWith(target) && d.Name == name);
            if (targetName != null) dirs.Add(targetName);
            foreach (var subDir in subDirs)
            {
                dirs.AddRange(Find(subDir, target));
            }
            return dirs;
        }

        private static string GetDirectoryName(string path)
        {
            var index = path.LastIndexOfAny(Separators);
            return index == -1 ? path : path.Substring(index + 1);
        }

        private static string ValidateSeparators(string path)
        {
            for (int i = 1; i < Separators.Length; i++)
                path = path.Replace(Separators[i], Separators[0]);
            return path;
        }
    }
}
