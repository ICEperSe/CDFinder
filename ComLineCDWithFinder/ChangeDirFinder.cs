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

        private static readonly char[] InvalidSymbols = new[] {'%', '*', '|', '<', '>', '$'};

        public ChangeDirFinder(string curDir)
        {
            CurrentDirectory = new DirectoryInfo(curDir);
        }

        public string GetPathTo(string targetDir)
        {
            if(targetDir == string.Empty)
                throw new ArgumentException(nameof(targetDir));
            if(targetDir == CurrentDirectory.Name)
                return targetDir;
            return Find(targetDir).FullName;
        }

        private DirectoryInfo Find(string dirName)
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.RootDirectory.Name == dirName)
                    return drive.RootDirectory;
                var dir = CheckEachDir(drive.RootDirectory, (d) => d.Name == dirName);
                if (dir != null)
                    return dir;
            }

            return null;
        }

        private DirectoryInfo CheckEachDir(DirectoryInfo srcDir, Func<DirectoryInfo, bool> condition)
        {
            return srcDir.GetDirectories().FirstOrDefault(condition);
        }
    }
}
