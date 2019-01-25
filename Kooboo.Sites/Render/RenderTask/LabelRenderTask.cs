//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
    public class LabelRenderTask : IRenderTask
    {
        private string Key { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false; 
            }
        }

        public LabelRenderTask(string key)
        {
            this.Key = key;
        }
        public string Render(RenderContext context)
        {
            var label = context.WebSite.SiteDb().Labels.GetByNameOrId(this.Key);

            if (label != null)
            {
                return label.GetValue(context.Culture).ToString();
            }
            return this.Key;
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) }); 
        }
    }
}
