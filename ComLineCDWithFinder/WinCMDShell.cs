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
        private static bool IsConsoleFree = false;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32")]
        private static extern bool FreeConsole();  
    
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void Run(string target)
        {
            CheckConsoleState();
            var curDir = Environment.CurrentDirectory;
            var targetDirs = PathFinder.GetPathTo(curDir, target);
            if (targetDirs.Length == 0)
            {
                Console.WriteLine("There is no such directory");
                return;
            }
            if (targetDirs.Length == 1)
            {
                WriteResult(targetDirs[0]);
            }
            else
            {
                if(SelectSingleOption(targetDirs, out var path))
                    WriteResult(path);
            }
        }

        private static bool SelectSingleOption(IReadOnlyList<string> options, out string path)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var i = 0;
            foreach (var dir in options)
            {
                Console.WriteLine($"{++i}. {dir}");
            }
            Console.Write("Enter number: ");
            if (int.TryParse(Console.ReadLine(), out var numb) 
                && numb <= options.Count && numb > 0)
            {
                path = options[numb - 1];
                return true;
            }

            path = null;
            return false;
        }

        private static void WriteResult(string resultPath)
        {
            CheckConsoleState();
            var process = Process.GetProcessesByName("cmd");
            if (process.Length <= 0) return;
            var pId = process[0].Id;
            FreeConsole();
            if (!AttachConsole((uint) pId))
            {
                Console.WriteLine(Marshal.GetLastWin32Error());
                return;
            }
            else
            {
                SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                SendKeys.SendWait("cd " + resultPath);
            }

            IsConsoleFree = true;
        }

        private static void CheckConsoleState()
        {
            if (IsConsoleFree)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
