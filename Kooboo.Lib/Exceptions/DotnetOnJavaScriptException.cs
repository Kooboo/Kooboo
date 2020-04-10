using Jint.Parser;
using System;
using System.Collections.Generic;
using System.Text;

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
