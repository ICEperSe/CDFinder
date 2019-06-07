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
    class WinCommandShell : ICommandShell
    {
        private readonly string[] SpecialKeys = new[] {"{", "}","+","^","%","~", "(", ")"};

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32")]
        private static extern bool FreeConsole();  
    
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public void Write(string str)
        {
            Console.Write(str);
        }

        public string Read()
        {
            return Console.ReadLine();
        }

        public void PutCommandToLine(string command)
        {
            var process = Process.GetProcessesByName("cmd");
            if (process.Length <= 0) return;
            var pId = process[0].Id;
            FreeConsole();
            if (!AttachConsole((uint) pId))
            {
                Write(Marshal.GetLastWin32Error().ToString());
            }
            else
            {
                SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                SendKeys.SendWait(ValidateCommand(command) + "{ENTER}");
            }
        }

        private string ValidateCommand(string command)
        {
            var strB = new StringBuilder(command);
            foreach (var key in SpecialKeys)
            {
                strB.Replace(key, "{" + key + "}");
            }

            return strB.ToString();
        }
    }
}
