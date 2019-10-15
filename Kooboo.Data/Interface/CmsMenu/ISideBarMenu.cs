//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Web.Menus
{
    public interface ISideBarMenu : ISitePermissionMenu
    {
        SideBarSection Parent { get; }
    }
}