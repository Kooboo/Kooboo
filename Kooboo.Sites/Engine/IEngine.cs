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
  public   interface IEngine
    {
        // for now, name and extension are the same...MUST be the same... 
        string Name { get; } 
        
        // Keep the tag like <script engine=kscript> ==> <script>
        bool KeepTag { get; }

        string Execute(RenderContext context, string input);

        bool IsScript { get; }

        bool IsStyle { get;  }
                                                    
        string Extension { get;  }

    }
}
