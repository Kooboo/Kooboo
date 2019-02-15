//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Commands
{
    public interface ICommandSourceProvider
    {
        // view or page.. 
        string GetString(RenderContext context, string RelativeUrl);

        byte[] GetBinary(RenderContext context, string RelativeUrl);

        // only for layout command..
        string GetLayout(RenderContext context, string RelativeUrl);

        System.IO.Stream GetStream(RenderContext context, string RelativeUrl);
        
    }
}
