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
    // pure output the original text. 
    public class Text : IEngine
    {
        public string Name { get { return "text";  } }
       
        public bool KeepTag
        {
            get { return true;  }
        }

        public string Extension => "text";

        public bool IsScript => false;

        public bool IsStyle => false; 

        public string Execute(RenderContext context, string input)
        {
            return input; 
        }
    }
}
