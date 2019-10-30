//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public interface IRenderTask
    {
        // only for #layout command that needs to clear all values before.. and insert into layout position.
        bool ClearBefore { get; }

        /// <summary>
        /// For render plans.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string Render(RenderContext context);

        void AppendResult(RenderContext context, List<RenderResult> result);
    }
}