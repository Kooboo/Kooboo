//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Data.Interface
{
    public interface IDashBoard
    {
        string Name { get; }

        string DisplayName(RenderContext context);

        IDashBoardResponse Render(RenderContext context);
    }
}