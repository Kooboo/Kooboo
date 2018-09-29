//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class NowFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "Now"; 
            }
        }

        public List<IFunction> Parameters
        {
            get;set;
        }

        public object Render(RenderContext context)
        {
            return DateTime.Now; 
        }
    }
     
    public class UtcNowFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "UtcNow";
            }
        }

        public List<IFunction> Parameters
        {
            get; set;
        }

        public object Render(RenderContext context)
        {
            return DateTime.UtcNow;
        }
    }
}
