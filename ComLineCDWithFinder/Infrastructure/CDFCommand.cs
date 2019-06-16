using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComLineCDWithFinder.Algorithm;

namespace ComLineCDWithFinder.Infrastructure
{
    class CDFCommand
    {
        private readonly ICommandShell _commandShell;

        private const string Command = "cd";
        private const string Help = @"Command line change_directory_finder
                                        -count=intNumb -> output intNumb paths
			                            -all ->  output all paths
			                            -s -> (default) output first path
			                            -i -> (default) ignore case
			                            -c -> case dependent search
			                            -h -> help";

        public CDFCommand(ICommandShell commandShell)
        {
            _commandShell = commandShell;
        }

        public void Execute(string[] args)
        {
            if(args is null) throw new ArgumentNullException();
            if(args.Length == 0) throw new ArgumentException(nameof(args));

            var options = FlagsProvider.GetOptions(args);
            if(options.Contains(Option.Undefined))
                throw new ArgumentException("Invalid flags");

            if (options.Contains(Option.Help))
            {
                OutputHelp();
            }
            else
                OutputResultAndSelect(
                    GetTargetPaths(Environment.CurrentDirectory,options, args)
                    );
        }

        private void OutputHelp()
        {
            _commandShell.Write(Help);
        }

        private void OutputResultAndSelect(string[] paths)
        {
            if (paths.Length == 0)
            {
                _commandShell.Write("There is no such directory");
                return;
            }
            if (paths.Length == 1)
            {
                _commandShell.PutCommandToLine(Command + " " + paths[0]);
            }
            else
            {
                if(SelectSingleOption(paths, out var path))
                    _commandShell.PutCommandToLine(Command + " " +path);
            }
        }


        private string[] GetTargetPaths(string curDir,Option[] options, string[] args)
        {
            var target = args.FirstOrDefault(s => !s.StartsWith("-"));
            if(target is null) 
                throw new ArgumentException("Target is not there: " + nameof(args));

            var ignoreCase = options.Contains(Option.IgnoreCase);
            int? count = null;

            if (options.Contains(Option.OutputCount))
                count = FlagsProvider.GetCountForCountFlag(args);
            else if (options.Contains(Option.OutputSingle))
                count = 1;

            return PathFinder.GetPaths(curDir, target, ignoreCase, count);
        }

        private bool SelectSingleOption(IReadOnlyList<string> options, out string path)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            OutputCollectionWithNumbers(options);

            _commandShell.Write("Enter number: ");
            if (
                int.TryParse(_commandShell.Read(), out var numb) 
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
            _commandShell.Write(strBuilder.ToString());
        }
    }
}
