//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor
{
    public interface IInlineExecutor
    {
        string EditorType
        { get; }

        void Execute(RenderContext context, List<IInlineModel> updatelist);

        void ExecuteObject(RenderContext context, IRepository repo, string nameOrId, List<IInlineModel> updates);
    }
}