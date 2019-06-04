using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Contents
{
    public class HtmlBlocks : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Contents;

        public string Name => "HtmlBlocks";

        public string Icon => "";

        public string Url => "Contents/HtmlBlocks";

        public int Order => 4;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("HtmlBlocks", Context); 
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("HtmlBlocks", context), Url = AdminUrl("Contents/HtmlBlocks", siteDb) ,  ActionRights = Sites.Authorization.Actions.Contents.HtmlBlocks}