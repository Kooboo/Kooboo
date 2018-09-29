//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.ScriptDebugger
{
    public class DebugSession
    { 
        public Guid CodeId { get; set; }

        // for the embedded js code.
        public int BodyHash { get; set; }

        public string IpAddress { get; set; }

        public DateTime ActiveTime { get; set; } 
         
        public Jint.Engine JsEngine { get; set; }
        
        public List<int> BreakLines { get; set; } = new List<int>(); 
        
        public ClientAction Action { get; set; }

        public DebugInfo DebugInfo { get; set; } 

        public bool EndOfSession { get; set; }

    }

    public class ClientAction
    {
        public  Jint.Runtime.Debugger.StepMode StepMode { get; set; }
         
    }
}
