using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Render;
using Kooboo.Sites.Render.Components;

namespace Kooboo.Render.Components
{

    public class ServerComponentRenderTask : IRenderTask
    {
        public ComponentSetting setting;

        public Dictionary<string, string> data { get; set; }

        public ServerComponentRenderTask(Element element)
        {
            this.setting = ComponentSetting.LoadFromElement(element);
            if (this.setting.Settings.Any() || this.setting.TagAttributes.Any())
            {
                data = new Dictionary<string, string>();

                foreach (var item in this.setting.TagAttributes)
                {
                    data.Add(item.Key, item.Value);
                }

                foreach (var item in this.setting.Settings)
                {
                    data[item.Key] = item.Value;
                }


            }
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
            var component = ComponentService.GetComponent(setting.TagName);

            if (this.data != null)
            {
                context.DataContext.Push(setting.Settings);
            }

            string result = Kooboo.Sites.Render.RenderHelper.Render(component.RenderTasks, context);

            if (this.data != null)
            {
                context.DataContext.Pop();
            }

            return result;
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }
    }


}
