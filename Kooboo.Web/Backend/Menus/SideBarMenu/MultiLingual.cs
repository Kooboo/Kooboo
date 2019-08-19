using System;
using System.Collections.Generic;
using System.Linq;  
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Menus.SideBarMenu
{
    public class MultiLingual : ISideBarMenu, IDynamicMenu
    {
        public SideBarSection Parent =>  SideBarSection.Root;

        public string Name => "Multilingual";

        public string Icon => "icon glyphicon glyphicon-globe";

        public string Url => "";

        public int Order =>  5;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Multilingual", Context); 
        }

        public bool Show(RenderContext context)
        {
            return context.WebSite != null && context.WebSite.EnableMultilingual; 
        }

        public List<ICmsMenu> ShowSubItems(RenderContext context)
        { 
            var siteDb = context.WebSite.SiteDb(); 
             
            List<string> othercultures = new List<string>();
            foreach (var item in siteDb.WebSite.Culture.Keys.ToList())
            {
                if (item.ToLower() != siteDb.WebSite.DefaultCulture.ToLower())
                {
                    othercultures.Add(item);
                }
            }

            List<ICmsMenu> result = new List<ICmsMenu>();

            foreach (var item in othercultures)
            {
                var cultureItem = new GeneralMenu() { Name = item };

                var contents = new GeneralMenu()
                {
                    Name = Hardcoded.GetValue("Contents", context)
                };

                var folders = siteDb.ContentFolders.All();

                foreach (var folder in folders)
                {
                    var folderMenu = new GeneralMenu();
                    folderMenu.Name = folder.DisplayName;
                    folderMenu.Url = "Multilingual/TextContentsByFolder?folder=" + folder.Id.ToString() + "&lang=" + item; 
                    contents.SubItems.Add(folderMenu); 
                }

                cultureItem.SubItems.Add(contents); 

                cultureItem.SubItems.Add(new GeneralMenu()
                {
                    Name = Hardcoded.GetValue("Labels", context),
                    Url = "Multilingual/Labels?lang=" + item
                });
                 
                cultureItem.SubItems.Add(new GeneralMenu()
                {
                    Name = Hardcoded.GetValue("HtmlBlocks", context),
                    Url = "Multilingual/HtmlBlocks?Lang=" + item
                });

                result.Add(cultureItem); 
            }

            return result; 
        }
    }
}

 