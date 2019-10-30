//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Render;

namespace Kooboo.Sites.App
{
    public static class AppContext
    {
        public static RenderContext GetAppContext(FrontContext context, string appId)
        {
            //clone and build a context for APP.
            string url = context.RenderContext.Request.RelativeUrl;
            return null;
        }
    }
}