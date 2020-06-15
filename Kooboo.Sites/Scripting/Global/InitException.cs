using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
  public class InitException : System.Exception
    {
        private string err { get; set; }
        public InitException(string message)
        {
            this.err = message; 
        }

        public override string Message => this.err; 
    }
}
