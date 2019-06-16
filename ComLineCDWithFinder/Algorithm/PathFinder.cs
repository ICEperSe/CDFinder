using System;
using System.IO;
using System.Linq;

namespace ComLineCDWithFinder.Algorithm
{
    internal static class PathFinder
    {
        public const string Asterisk = "*";
        private static readonly char[] InvalidSymbols = { '%', '|', '<', '>', '$' };

        public static string[] GetPaths(string curDir, string targetDir)
        {
            return GetPaths(curDir, targetDir, false, null);
        }

        public static string[] GetPaths(string curDir, string targetDir, bool ignoreCase)
        {
            return GetPaths(curDir, targetDir, ignoreCase, null);
        }

        public static string[] GetPaths(string curDir, string targetDir, bool ignoreCase, int? count)
        {
            if(curDir == null) throw new ArgumentNullException(nameof(curDir));
            if(targetDir == null) throw new ArgumentNullException(nameof(targetDir));
            if (!Directory.Exists(curDir))throw new ArgumentException();
            if (targetDir == string.Empty || targetDir.IndexOfAny(InvalidSymbols) != -1)
                throw new ArgumentException(nameof(targetDir));
            
            targetDir = ValidateSeparators(targetDir);
            var startDir = new DirectoryInfo(curDir);
            if (targetDir == startDir.Name)
                return new[] { startDir.FullName };

            if (targetDir.StartsWith(Path.DirectorySeparatorChar.ToString()))
                targetDir = Asterisk + targetDir;

            if (Path.IsPathRooted(targetDir))
            {
                if (Directory.Exists(targetDir))
                    return new[] { targetDir };
                else
                    startDir = new DirectoryInfo(Path.GetPathRoot(targetDir));
            }

            var controller = new DirSearchController(
                DirectorySearchRuleProvider.GetRule(targetDir, ignoreCase), count??int.MaxValue);

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
                    if (!controller.IsEnd)
                    {
                        controller.GetItem(subDir);
                        GoThroughSubDirs(subDir, controller);
                    }
                    else 
                        break;
                    
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