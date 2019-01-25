//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render
{
    public class ContentRenderTask : IRenderTask
    {
        public string Content;

        public ContentRenderTask(string content)
        {
            Content = content;
        }

        public bool ClearBefore
        {
            get
            {
                return false; 
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Content }); 
        }

        public string Render(RenderContext context)
        {
            return Content;
        }
    }
}
