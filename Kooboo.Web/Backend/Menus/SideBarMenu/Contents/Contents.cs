using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Menus.SideBarMenu.Contents
{
    public class Contents : ISideBarMenu, IDynamicMenu
    {
        public SideBarSection Parent =>  SideBarSection.Contents;

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

        public List<ICmsMenu> ShowSubItems(RenderContext context)
        {
            if (context.WebSite == null)
            {
                return null;
            }

            var sitedb = context.WebSite.SiteDb();

            var folders = sitedb.ContentFolders.All();

            List<ICmsMenu> result = new List<ICmsMenu>();

            foreach (var item in folders)
            { 
                var model = new GeneralMenu();
                model.Name = item.DisplayName;
                model.Url = "Contents/TextContentsByFolder?folder=" + item.Id.ToString();

                result.Add(model);  
            }

            return result; 
        } 

    }
}

//  new MenuItem
                    //{
                    //    Name = Hardcoded.GetValue("Contents", context),
                    //    Url = AdminUrl("Contents/TextContents", siteDb),
                    //    Items = SiteMenu_SubContent(user, siteDb),  ActionRights = Sites.Authorization.Actions.Contents.Content
                    //},