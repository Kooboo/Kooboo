using System;
using System.Collections.Generic;

namespace Kooboo.HttpServer
{
    public class Utility
    {
        public static bool IsFoldingLine(string line)
        {
            if (line == null)
                throw new ArgumentException("line", "Must not be null");

            return line.StartsWith(" ") || line.StartsWith("\t");
        }

        public static KeyValuePair<string, string> SplitHeaderLine(string line)
        {
            var index = line.IndexOf(':');
            if (index == 0)
                throw new ArgumentException("line", "Must contains name");

            if (index < 0 || index == line.Length - 1)
                return new KeyValuePair<string, string>(line, null);

            return new KeyValuePair<string, string>(line.Substring(0, index), line.Substring(index + 1));
        }
    }
}