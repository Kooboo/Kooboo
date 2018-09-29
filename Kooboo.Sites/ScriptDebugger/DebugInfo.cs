//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Jint.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.ScriptDebugger
{
  public  class DebugInfo
    {
        public bool HasValue { get; set; } = true;

        public bool EndOfExe { get; set; } = false;

        public bool IsException { get; set; } = false; 

        public string Message { get; set; }
          
        public Position Start;

        public Position End;
        
        public DebugVariables Variables { get; set; }

    }

    public class DebugVariables
    {
        public Dictionary<string, object> Local = new Dictionary<string, object>();
        public Dictionary<string, object> Global = new Dictionary<string, object>(); 

    }

    public class ExeResult 
    {
        public DebugVariables Variables { get; set; }

        public bool Success { get; set; }

        public string Model { get; set; }
              
    }


}
