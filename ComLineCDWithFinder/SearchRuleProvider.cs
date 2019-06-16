using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    static class DirectorySearchRuleProvider
    {
        public const string Asterisk = "*";

        private static readonly char[] InvalidSymbols = {'%', '|', '<', '>', '$'};

        private static readonly char[] Separators =
            {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};

        public static Predicate<DirectoryInfo> GetRule(string target, bool ignoreCase)
        {
            if (target.Contains(Asterisk)) 
                return GetRule_ForAsterisk(target, ignoreCase);

            if (target.IndexOfAny(Separators) == -1)
                return GetRule_ForSimpleName(target, ignoreCase);
            else
                return GetRule_ForPathPart(target, ignoreCase);

        }

        private static Predicate<DirectoryInfo> GetRule_ForAsterisk(string target, bool ignoreCase)
        {
            if(!Path.IsPathRooted(target))
                target = Path.DirectorySeparatorChar + target;
            target = Regex.Escape(target).Replace("\\" + Asterisk,"[^/\\\\]" + Asterisk);
            var option = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            var regex = new Regex("." + Asterisk + target + "$", option);
            return d => regex.IsMatch(d.FullName);
        }

        private static Predicate<DirectoryInfo> GetRule_ForSimpleName(string target, bool ignoreCase)
        {
            var option = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return d => d.Name.Equals(target,option);
        }

        private static Predicate<DirectoryInfo> GetRule_ForPathPart(string target, bool ignoreCase)
        {
            var name = GetDirectoryName(target);
            var option = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return d => d.FullName.EndsWith(target, option) && d.Name.Equals(name,option);
        }

        private static string GetDirectoryName(string path)
        {
            var index = path.LastIndexOfAny(Separators);
            return index == -1 ? path : path.Substring(index + 1);
        }
        
    }
}
