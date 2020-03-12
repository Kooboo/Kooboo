using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace VirtualFile
{
    static class Helper
    {
        public static string NormalizePath(string path)
        {
            path = Path.GetFullPath(path);
            path = new Uri(path).LocalPath;
            return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .ToUpperInvariant();
        }

        public static string GetWildcardRegexString(string wildcardStr)
        {
            var replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            return replace.Replace(wildcardStr,
                 delegate (Match m)
                 {
                     switch (m.Value)
                     {
                         case "?":
                             return ".?";
                         case "*":
                             return ".*";
                         default:
                             return "\\" + m.Value;
                     }
                 });
        }
    }
}
