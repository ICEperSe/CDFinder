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

        public string GetPathTo(string targetDir)
        {
            if(targetDir == string.Empty)
                throw new ArgumentException(nameof(targetDir));
            if(targetDir == CurrentDirectory.Name)
                return CurrentDirectory.FullName;
            if (Path.IsPathRooted(targetDir) && Directory.Exists(targetDir)) return targetDir;
            return Find(CurrentDirectory, targetDir)?.FullName;
        }

        private DirectoryInfo Find(DirectoryInfo parent, string dirName)
        {
            foreach (var subDir in parent.GetDirectories())
            {
                if (subDir.Name == dirName) return subDir;
            }
            foreach (var subDir in parent.GetDirectories())
            {
                var res = Find(subDir, dirName);
                if (res != null) return res;
            }
            return null;
        }
    }
}
