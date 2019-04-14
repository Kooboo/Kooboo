using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render
{
    [Serializable]
    public class ViewParseException : Exception
    {
        public ViewParseException() { }
        public ViewParseException(string message) : base(message) { }
        public ViewParseException(string message, Exception inner) : base(message, inner) { }
        protected ViewParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
