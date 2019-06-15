using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ComLineCDWithFinder
{
    class WinCommandShell : ICommandShell
    {
        private readonly string[] SpecialKeys = {"{", "}","+","^","%","~", "(", ")"};

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
            var process = GetParentProcess(Process.GetCurrentProcess());
            var pId = process.Id;
            FreeConsole();
            if (!AttachConsole((uint) pId))
            {
                Write(
                    Marshal.GetLastWin32Error().ToString()
                    );
            }
            else
            {
                SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                SendKeys.SendWait(
                    ValidateCommand(command) + "{ENTER}"
                    );
            }
        }

        private Process GetParentProcess(Process curProcess)
        {
            var query = $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {curProcess.Id}";
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            results.MoveNext();
            var queryObj = results.Current;
            return Process.GetProcessById((int)(uint)queryObj["ParentProcessId"]);
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
