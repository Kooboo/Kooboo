//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Web.Menus
{
    public interface ICmsMenu
    {
        string Name { get; }

        string GetDisplayName(RenderContext context);

        string Icon { get; }

        string Url { get; }

        int Order { get; }

        List<ICmsMenu> SubItems { get; set; }
    }
}