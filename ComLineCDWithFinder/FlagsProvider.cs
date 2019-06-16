using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    static class FlagsProvider
    {
        public static readonly string[] Flags = 
            new[] {"-count=", "-all", "-s", "-i", "-c", "-h"};

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
                    case string str when arg.Contains("-count="):
                        options.Add(Option.OutputCount);
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

        private static bool AreOptionsValid(IEnumerable<Option> options)
        {
            return !options.Select(o1 => options.Any(o2=>(o1&o2) != 0 && o1!=o2)).Any(e=>e);
        }
    }

}
