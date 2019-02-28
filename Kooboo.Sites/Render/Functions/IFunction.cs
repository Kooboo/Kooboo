//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Functions
{
    public interface IFunction
    {
        string Name { get; }

        List<IFunction> Parameters { get; set; }
         
        object Render(RenderContext context); 
    }
}
