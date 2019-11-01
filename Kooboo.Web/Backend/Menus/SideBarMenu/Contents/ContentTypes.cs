using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

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

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("ContentTypes", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("ContentTypes", context), Url = AdminUrl("Contents/ContentTypes", siteDb) ,  ActionRights = Sites.Authorization.Actions.Contents.ContentTypes}