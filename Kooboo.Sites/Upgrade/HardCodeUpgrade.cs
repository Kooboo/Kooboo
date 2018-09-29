//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Events.Global;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Upgrade
{
    public class HardCodeUpgrade : Data.Events.IHandler<ApplicationStartUp>
    {
        //public void Handle(ApplicationStartUp theEvent, RenderContext context)
        //{
        //    //var upgrade = new UpgradeMenuTextContent();
        //    //System.Threading.Thread thread = new System.Threading.Thread(upgrade.Start);
        //    //thread.Start();  

        //   var list =  UpgradeManager.ListUpgraders();

        //    foreach (var item in list)
        //    {
        //        item.Do(); 
        //    }  
        //    // update the latest version to globalsetting.  
        //} 
        public void Handle(ApplicationStartUp theEvent, RenderContext context)
        {
           // throw new NotImplementedException();
        }
    }

    public class UpgradeMenuTextContent
    {
        public void Start()
        {
            var sites = Kooboo.Data.GlobalDb.WebSites.All().ToList(); 
            // correct menu.. 
            foreach (var site in sites)
            {
                var sitedb = site.SiteDb();
                string culture = site.DefaultCulture; 

                var allmenus = sitedb.Menus.All();
                foreach (var menu in allmenus)
                {
                    if (menu.Values.Count()==0)
                    {
                         if (!string.IsNullOrEmpty(menu.Name))
                        {
                            menu.Values.Add(culture, menu.Name);
                            sitedb.Menus.AddOrUpdate(menu); 
                        }
                    }
                } 
                // correct textcontent cultulre 
                var allcontents = sitedb.TextContent.All();  
                foreach (var item in allcontents)
                {
                    bool haschange = false; 
                    foreach (var content in item.Contents)
                    {
                        if (content.Lang.Length >2)
                        {
                            content.Lang = content.Lang.Substring(0, 2);
                            content.Lang = content.Lang.ToLower();
                            haschange = true; 
                        }
                    }
                    if (haschange)
                    {
                        sitedb.TextContent.AddOrUpdate(item);
                    }
                }
                 
                // update site culture. 
                bool sitechange = false;
                foreach (var item in site.Cultures)
                { 
                    if (!string.IsNullOrEmpty(item))
                    {
                        string value = item; 
                        if (value.Length>2)
                        {
                            value = value.Substring(0, 2);

                            if (!site.Culture.ContainsKey(value))
                            {
                                string name = value;

                                if (Kooboo.Data.Language.LanguageSetting.ISOTwoLetterCode.ContainsKey(value))
                                {
                                    name = Data.Language.LanguageSetting.ISOTwoLetterCode[value];
                                }
                                site.Culture[value] = name;
                                sitechange = true; 
                            }

                        } 
                    } 
                }

                if (sitechange)
                {
                    Data.GlobalDb.WebSites.AddOrUpdate(site); 
                }
            } 
        }
    }

}
