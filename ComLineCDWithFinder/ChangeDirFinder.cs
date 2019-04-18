using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    public class ChangeDirFinder
    {
        public DirectoryInfo CurrentDirectory;

        public ChangeDirFinder(string curDir)
        {
            CurrentDirectory = new DirectoryInfo(curDir);
        }

        public string GetPathTo(string targetDir)
        {
            if(targetDir == string.Empty)
                throw new ArgumentException(nameof(targetDir));
            //if(targetDir == CurrentDirectory.Name)
            return null;
        }
    }
}
