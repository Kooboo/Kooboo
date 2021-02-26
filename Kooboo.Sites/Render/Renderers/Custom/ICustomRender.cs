using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Renderers
{
   public interface ICustomRender
    {
        string Name { get;   }
        Task RenderAsync (FrontContext context, string parameter); 
    }
}
