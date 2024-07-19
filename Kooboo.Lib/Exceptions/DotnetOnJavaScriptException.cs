using System;
using Esprima;

namespace Kooboo.Lib.Exceptions
{
    public class DotnetOnJavaScriptException : Exception
    {
        public DotnetOnJavaScriptException(Location location, string message, Exception innerException) : base(message, innerException)
        {
            Location = location;
        }

        public Location Location { get; set; }
    }
}
