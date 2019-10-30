//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Commands
{
    public interface ICommandSourceProvider
    {
        // view or page..
        string GetString(RenderContext context, string relativeUrl);

        byte[] GetBinary(RenderContext context, string relativeUrl);

        // only for layout command..
        string GetLayout(RenderContext context, string relativeUrl);

        System.IO.Stream GetStream(RenderContext context, string relativeUrl);
    }
}