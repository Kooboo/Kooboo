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
    public class DebugInfo
    {
        public int CurrentLine;

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
