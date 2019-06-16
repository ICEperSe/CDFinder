using System;
using System.IO;
using System.Linq;

namespace ComLineCDWithFinder.Algorithm
{
    internal static class PathFinder
    {
        public const string Asterisk = "*";
        private static readonly char[] InvalidSymbols = { '%', '|', '<', '>', '$' };

        public static string[] GetPath(string curDir, string targetDir)
        {
            return GetPaths(curDir, targetDir, false, int.MaxValue);
        }

        public static string[] GetPath(string curDir, string targetDir, bool ignoreCase)
        {
            return GetPaths(curDir, targetDir, ignoreCase, int.MaxValue);
        }

        public static string[] GetPaths(string curDir, string targetDir, bool ignoreCase, int count)
        {
            if(curDir == null) throw new ArgumentNullException(nameof(curDir));
            if(targetDir == null) throw new ArgumentNullException(nameof(targetDir));

            if (!Directory.Exists(curDir))
                throw new ArgumentException();


            targetDir = ValidateSeparators(targetDir);
            if (targetDir == string.Empty || targetDir.IndexOfAny(InvalidSymbols) != -1)
                throw new ArgumentException(nameof(targetDir));

            var startDir = new DirectoryInfo(curDir);
            if (targetDir == startDir.Name)
                return new[] { startDir.FullName };

            if (Path.IsPathRooted(targetDir))
            {
                if (Directory.Exists(targetDir))
                    return new[] { targetDir };
                else
                    startDir = new DirectoryInfo(Path.GetPathRoot(targetDir));
            }

            var controller = new DirSearchController(
                DirectorySearchRuleProvider.GetRule(targetDir, ignoreCase), count);

            GoThroughSubDirs(startDir, controller);
            return controller.FoundedItems.Select(d=>d.FullName).ToArray();
        }

        private static void GoThroughSubDirs(DirectoryInfo parentDirectory,
                                             ISearchController<DirectoryInfo> controller)
        {
            try
            {
                foreach (var subDir in parentDirectory.GetDirectories())
                {
                    controller.GetItem(subDir);
                    if (!controller.IsEnd)
                        GoThroughSubDirs(subDir, controller);
                }
            }
            catch
            {
                // ignored
            }
        }

        private static string ValidateSeparators(string path)
            => path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }
}