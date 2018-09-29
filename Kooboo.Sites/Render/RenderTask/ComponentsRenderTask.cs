//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Render.Components;
using Kooboo.Dom;

namespace Kooboo.Sites.Render
{
    public class ComponentRenderTask : IRenderTask
    {
        public ComponentSetting setting;

        public ComponentRenderTask(Element element)
        {
            this.setting = ComponentSetting.LoadFromElement(element);
        }

        public bool ClearBefore
        {
            get
            {
                return false; 
            }
        }

        public string Render(RenderContext context)
        {
            var component = Container.Get(setting.TagName);
            Task<string> task = component.RenderAsync(context, setting);
            if (task == null)
            {
                return null; 
            }
           else
            {
                return task.Result; 
            }      
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) }); 
        }
    }
}
