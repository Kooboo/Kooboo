using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Contents
{
    public class Labels : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Contents;

        public string Name => "Labels";

        public string Icon => "";

        public string Url => "Contents/Labels";

        public int Order => 3;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Labels", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Labels", context), Url = AdminUrl("Contents/Labels", siteDb),  ActionRights = Sites.Authorization.Actions.Contents.Labels }