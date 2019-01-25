//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Engine
{
   public class KForm : IEngine
    {
         
        public string Name { get { return "kform"; } }

        public bool KeepTag { get { return false; } }

        public string Extension { get => "kform";   }

        public bool IsScript => false;

        public bool IsStyle => false; 

        public string Execute(RenderContext context, string input)
        {
            return null; // looks like this will not be executed, only to make an definition for component evaluator.  
            //return Kooboo.Sites.Scripting.Manager.ExecuteCode(context, input);
        } 
    }
}
