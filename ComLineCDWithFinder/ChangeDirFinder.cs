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
                return CurrentDirectory.FullName;
            var res = CurrentDirectory
                .GetDirectories()
                .FirstOrDefault(d => d.Name == targetDir);
            return res?.FullName;
        }

        private DirectoryInfo Find(string dirName)
        {

            return null;
        }
    }
}
