using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Contents
{
    public class Contents : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Contents;

        public string Name => "Contents";

        public string Icon => "";

        public string Url => "Contents/TextContents";

        public int Order => 1;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Contents", Context);
        }

        public bool Show(RenderContext context)
        {
            return true;
        }

    }
}

//  new MenuItem
//{
//    Name = Hardcoded.GetValue("Contents", context),
//    Url = AdminUrl("Contents/TextContents", siteDb),
//    Items = SiteMenu_SubContent(user, siteDb),  ActionRights = Sites.Authorization.Actions.Contents.Content
//},