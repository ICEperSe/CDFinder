using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    class CDFCommand
    {
        private readonly ICommandShell CommandShell;

        private const string Command = "cd";

        public CDFCommand(ICommandShell commandShell)
        {
            CommandShell = commandShell;
        }

        public void Run(string[] args)
        {
            var curDir = Environment.CurrentDirectory;
            var targetDirs = GetTargetDirs(curDir, args);
            if (targetDirs.Length == 0)
            {
                CommandShell.Write("There is no such directory");
                return;
            }
            if (targetDirs.Length == 1)
            {
                CommandShell.PutCommandToLine(Command + " " + targetDirs[0]);
            }
            else
            {
                if(SelectSingleOption(targetDirs, out var path))
                    CommandShell.PutCommandToLine(Command + " " +path);
            }
        }

        private string[] GetTargetDirs(string curDir, string[] options)
        {
            return PathFinder.GetPath(curDir, options[0]);
        }

        private bool SelectSingleOption(IReadOnlyList<string> options, out string path)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            OutputCollectionWithNumbers(options);

            CommandShell.Write("Enter number: ");
            if (
                int.TryParse(CommandShell.Read(), out var numb) 
                && numb <= options.Count 
                && numb > 0)
            {
                path = options[numb - 1];
                return true;
            }

            path = null;
            return false;
        }

        private void OutputCollectionWithNumbers(IReadOnlyList<string> collection)
        {
            var i = 0;
            var strBuilder = new StringBuilder();
            foreach (var dir in collection)
            {
                strBuilder.AppendLine($" {++i}. {dir}");
            }
            CommandShell.Write(strBuilder.ToString());
        }
    }
}
