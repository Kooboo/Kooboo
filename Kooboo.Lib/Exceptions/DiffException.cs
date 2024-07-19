using System;

namespace Kooboo.Lib.Exceptions
{
    public class DiffException : Exception
    {
        public DiffException(long version, string body)
        {
            Version = version;
            Body = body;
        }

        public long Version { get; }
        public string Body { get; }
    }
}

