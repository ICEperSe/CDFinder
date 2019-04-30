using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComLineCDWithFinder
{
    static class WinCMDShell
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32")]
        static extern bool FreeConsole();  
    
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void Run(string target)
        {
            var curDir = Environment.CurrentDirectory;
            var targetDirs = PathFinder.GetPathTo(curDir, target);
            if (targetDirs.Length == 0) Console.WriteLine("There is no such directory");
            if (targetDirs.Length == 1)
            {
                Process[] procs = Process.GetProcessesByName("cmd");
                if (procs.Length > 0)
                {
                    int pId = procs[0].Id;
                    FreeConsole();
                    bool ok = AttachConsole((uint) pId);
                    if (!ok)
                    {
                        int error = Marshal.GetLastWin32Error();
                        Console.WriteLine(error);
                    }
                    else
                    {
                        SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                        SendKeys.SendWait("cd " + targetDirs[0]);
                    }
                }
            }
        }
    }
}
