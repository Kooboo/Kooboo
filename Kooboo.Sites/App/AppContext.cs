using Kooboo.Data.Context;
using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.App
{
    public static class AppContext
    {    
        public static RenderContext  GetAppContext(FrontContext context, string AppId)
        {
            //clone and build a context for APP. 
            string url = context.RenderContext.Request.RelativeUrl;   
            return null; 
        }   
    }
}
