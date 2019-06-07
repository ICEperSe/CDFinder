using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                new CDFCommand(new WinCommandShell()).Run(args);
            }
        }
    }
}
