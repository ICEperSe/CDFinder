using System;
using System.Collections.Generic;
using System.Linq;

namespace ComLineCDWithFinder.Infrastructure
{
    static class FlagsProvider
    {
        public static readonly string[] Flags = {"-count=", "-all", "-s", "-i", "-c", "-h"};

        public static Option[] GetOptions(params string[] args)
        {
            var options = new List<Option>();
            foreach (var arg in args)
            {
                if(arg.StartsWith("-"))
                switch (arg)
                {
                    case "-all":
                        options.Add(Option.OutputAll);
                        break;
                    case "-s":
                        options.Add(Option.OutputSingle);
                        break;
                    case "-i":
                        options.Add(Option.IgnoreCase);
                        break;
                    case "-c":
                        options.Add(Option.CaseSensitive);
                        break;
                    case "-h":
                        options.Add(Option.Help);
                        break;
                    case string countFlag when arg.Contains("-count="):
                        countFlag = arg.Substring(arg.IndexOf("=", StringComparison.Ordinal)+1);
                        if(int.TryParse(countFlag, out _))
                            options.Add(Option.OutputCount);
                        else options.Add(Option.Undefined);
                        break;
                    default:
                        options.Add(Option.Undefined);
                        break;
                }
            }
            if(AreOptionsValid(options))
                return options.ToArray();
            return new[]{Option.Undefined};
        }

        public static int GetCountForCountFlag(params string[] args)
        {
            foreach (var flag in args)
            {
                if (flag.Contains("-count="))
                {
                    if (int.TryParse(flag.Substring(flag.IndexOf("=", StringComparison.Ordinal) + 1), out var count))
                    {
                        return count;
                    }
                    throw new ArgumentException("Invalid count flag");
                }
            }
            throw new ArgumentException("No count flag");
        }

        private static bool AreOptionsValid(IEnumerable<Option> options)
        {
            return !options.Select(o1 => options.Any(o2=>(o1&o2) != 0 && o1!=o2)).Any(e=>e);
        }
    }

}
