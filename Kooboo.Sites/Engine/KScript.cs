//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Engine
{
    public class KScript : IEngine
    {
        public string Name { get { return "kscript";  } }

        public bool KeepTag { get { return false; } }

        public string Extension => "kscript";

        public bool IsScript => false;

        public bool IsStyle => false; 

        public string Execute(RenderContext context, string input)
        {
            return Kooboo.Sites.Scripting.Manager.ExecuteCode(context, input); 
        }
    }
}
