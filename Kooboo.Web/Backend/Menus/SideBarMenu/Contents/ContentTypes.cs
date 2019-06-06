using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Contents
{
    public class ContentTypes : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Contents;

        public string Name => "ContentTypes";

        public string Icon => "";

        public string Url => "Contents/ContentTypes";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("ContentTypes", Context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("ContentTypes", context), Url = AdminUrl("Contents/ContentTypes", siteDb) ,  ActionRights = Sites.Authorization.Actions.Contents.ContentTypes}