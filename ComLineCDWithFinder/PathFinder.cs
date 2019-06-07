using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComLineCDWithFinder
{
    internal static class PathFinder
    {
        public const string Asterisk = "*";
        private static readonly char[] InvalidSymbols = {'%', '|', '<', '>', '$'};

        private static readonly char[] Separators =
            {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};

        public static string[] GetPathTo(DirectoryInfo curDir, string targetDir)
        {
            return GetPathTo(curDir.FullName, targetDir);
        }

        public static string[] GetPathTo(string curDir, string targetDir)
        {
            targetDir = ValidateSeparators(targetDir);
            if(!Directory.Exists(curDir))
                throw new ArgumentException();
            var currentDirectory = new DirectoryInfo(curDir);
            if (targetDir == string.Empty || targetDir.IndexOfAny(InvalidSymbols) != -1)
                throw new ArgumentException(nameof(targetDir));
            if (targetDir == currentDirectory.Name)
                return new[] {currentDirectory.FullName};
            if (Path.IsPathRooted(targetDir))
            {
                if(Directory.Exists(targetDir))
                    return new[] {targetDir};
                else 
                    currentDirectory = new DirectoryInfo(Path.GetPathRoot(targetDir));
            }
            return FindDirectories(currentDirectory, GetSearchRule(targetDir))
                    .Select(d => d.FullName)
                    .ToArray();
        }

        private static IEnumerable<DirectoryInfo> FindDirectories
            (DirectoryInfo parent, Predicate<DirectoryInfo> rule)
        {
            var subDirs = parent.GetDirectories();
            var dirs = new List<DirectoryInfo>();
            var targetFromParent = subDirs.Where(d=>rule(d));
            dirs.AddRange(targetFromParent);
            foreach (var subDir in subDirs)
            {
                try
                {
                    dirs.AddRange(FindDirectories(subDir, rule));
                }
                catch
                {
                    // ignored
                }
            }

            return dirs;
        }

        private static string GetDirectoryName(string path)
        {
            var index = path.LastIndexOfAny(Separators);
            return index == -1 ? path : path.Substring(index + 1);
        }

        private static string ValidateSeparators(string path)
            => path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        private static Predicate<DirectoryInfo> GetSearchRule(string target)
        {
            if(!target.Contains(Asterisk))
            {
                if (target.IndexOfAny(Separators) == -1)
                    return GetRule_ForSimpleName(target);
                else
                    return GetRule_ForPathPart(target);
            }
            
            return GetRule_ForAsterisk(target);
        }

        private static Predicate<DirectoryInfo> GetRule_ForAsterisk(string target)
        {
            if(!Path.IsPathRooted(target))
                target = Path.DirectorySeparatorChar + target;
            target = Regex.Escape(target).Replace("\\" + Asterisk,"[^/\\\\]" + Asterisk);
            var regex = new Regex("." + Asterisk + target + "$");
            return d => regex.IsMatch(d.FullName);
        }

        private static Predicate<DirectoryInfo> GetRule_ForSimpleName(string target)
        {
            return d => d.Name == target;
        }

        private static Predicate<DirectoryInfo> GetRule_ForPathPart(string target)
        {
            var name = GetDirectoryName(target);
            return d => d.FullName.EndsWith(target) && d.Name == name;
        }
    }
}