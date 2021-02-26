using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Renderers.Custom
{
    public static class CustomRenderManager
    {
        // static constructor
        static CustomRenderManager()
        {
            container = new Dictionary<string, ICustomRender>(StringComparer.OrdinalIgnoreCase);
            var list = Kooboo.Lib.IOC.Service.GetInstances<ICustomRender>();
            foreach (var item in list)
            { 
                container.Add(item.Name, item);
            }
        }

        public static Dictionary<string, ICustomRender> container
        {
            get; set;
        }

        public static bool HasType(string typename)
        {
            return container.ContainsKey(typename);
        }

        public static async Task RenderAsync(string TypeName, string Parameter, FrontContext context)
        {
            if (container.ContainsKey(TypeName))
            {
                var instance = container[TypeName];
                await instance.RenderAsync(context, Parameter); 
            }
        }

        public static string GetRoute<T>(string parameter) where T : ICustomRender
        {
            var instance = Activator.CreateInstance(typeof(T)) as ICustomRender;  
            var name = instance.Name;
            return "/__kb/custom/" + name + "/" + parameter;
        }
    }

}
