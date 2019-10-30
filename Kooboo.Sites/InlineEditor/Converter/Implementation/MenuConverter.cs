//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public class MenuConverter : IConverter, IContinueConverter
    {
        public string Type
        {
            get
            {
                return "menu";
            }
        }

        public ConvertResponse Convert(RenderContext context, JObject result)
        {
            var page = context.GetItem<Page>();
            var sitedb = context.WebSite.SiteDb();

            string menuname = Lib.Helper.JsonHelper.GetString(result, "name");
            menuname = GetMenuName(sitedb, menuname);
            var data = Lib.Helper.JsonHelper.GetObject(result, "Data");

            //var newmenu = Newtonsoft.Json.JsonConvert.DeserializeObject<Menu>(data.ToString());
            var newmenu = Lib.Helper.JsonHelper.Deserialize<Menu>(data.ToString());
            newmenu.Name = menuname;
            newmenu.Id = default(Guid);

            EnSureParentId(newmenu);

            string koobooid = Lib.Helper.JsonHelper.GetString(result, "KoobooId");

            if (string.IsNullOrEmpty(koobooid))
            {
                koobooid = DetectKoobooId(page.Dom, newmenu);
            }
            if (string.IsNullOrEmpty(koobooid))
            {
                return null;
            }

            var response = new ConvertResponse
            {
                IsSuccess = true,
                ComponentNameOrId = menuname,
                Tag = "<menu id='" + menuname + "'></menu>",
                KoobooId = koobooid
            };

            if (page == null)
            {
                sitedb.Menus.AddOrUpdate(newmenu);
                return response;
            }

            if (sitedb.WebSite.ContinueConvert)
            {
                var domNode = DomService.GetElementByKoobooId(page.Dom, koobooid);
                if (domNode is Element domElement)
                {
                    var elementpaths = Kooboo.Sites.Service.DomService.GetElementPath(domElement as Kooboo.Dom.Element);

                    sitedb.ContinueConverter.AddConverter(this.Type, page.Id, response.Tag, response.ComponentNameOrId, koobooid, elementpaths, domElement.tagName);
                    // var OtherPages = sitedb.Pages.All().Where(o => o.Id != page.Id && !o.HasLayout).ToList();
                    //foreach (var item in OtherPages)
                    //{
                    //    ContinueConvert(sitedb, page.Id, response.Tag, koobooid, item, null, DomElement.ParentPathHash, newmenu);
                    //}
                }
            }

            sitedb.Menus.AddOrUpdate(newmenu);

            // if (!sitedb.WebSite.ContinueConvert)
            // {
            //    sitedb.WebSite.ContinueConvert = true;
            //     Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(sitedb.WebSite);
            // }

            return response;
        }

        public void ContinueConvert(SiteDb siteDb, Guid originalPageId, string convertedTag, string objectNameOrId, string koobooId, Page currentPage, List<string> elementPath)
        {
            if (originalPageId == currentPage.Id)
            {
                return;
            }
            Menu menu = siteDb.Menus.GetByNameOrId(objectNameOrId);
            if (menu == null)
            {
                return;
            }
            var menuelement = DomService.GetElementByPath(currentPage.Dom, elementPath);

            if (menuelement != null)
            {
                var rawmenu = MenuService.FindRawMenu(menuelement);
                if (rawmenu != null)
                {
                    AssignRawMenu(menu, rawmenu);
                }
                currentPage.Body = currentPage.Body.Substring(0, menuelement.location.openTokenStartIndex) + convertedTag + currentPage.Body.Substring(menuelement.location.endTokenEndIndex + 1);

                siteDb.Pages.AddOrUpdate(currentPage);
            }
        }

        public void EnSureParentId(Menu menu)
        {
            if (menu.children != null && menu.children.Any())
            {
                foreach (var item in menu.children)
                {
                    item.ParentId = menu.Id;

                    EnSureParentId(item);
                }
            }
        }

        public void AssignRawMenu(Menu currentMenu, RawMenu raw)
        {
            foreach (var item in raw.Children)
            {
                if (item.LinkElement != null)
                {
                    var href = item.LinkElement.getAttribute("href");
                    var find = currentMenu.children.Find(o => o.Url == href);
                    if (find != null)
                    {
                        AssignRawMenu(find, item);
                    }
                    else
                    {
                        Menu newmenu = new Menu {Url = href, Name = item.LinkElement.InnerHtml};
                        currentMenu.children.Add(newmenu);
                        AssignRawMenu(newmenu, item);
                    }
                }
            }
        }

        public string FindSameKoobooId(SiteDb siteDb, Page destinationPage, Menu currentMenu, Guid parentPathHash)
        {
            var candidates = siteDb.DomElements.Query.Where(o => o.OwnerObjectId == destinationPage.Id && o.ParentPathHash == parentPathHash).SelectAll();

            foreach (var item in candidates)
            {
                if (VerifyAsMenu(siteDb, destinationPage, item, currentMenu))
                {
                    return item.KoobooId;
                }
            }

            return null;
        }

        internal bool VerifyAsMenu(SiteDb siteDb, Page page, DomElement foundElement, Menu currentMenu)
        {
            var koobooid = foundElement.KoobooId;

            var domelement = Service.DomService.GetElementByKoobooId(page.Dom, koobooid);

            if (domelement != null)
            {
                if (domelement is Element element)
                {
                    var rawmenu = Service.MenuService.FindRawMenu(element);

                    if (GuessSameMenu(currentMenu, rawmenu))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal bool GuessSameMenu(Menu menu, RawMenu rawmenu)
        {
            if (rawmenu != null)
            {
                if (rawmenu.LinkElement != null)
                {
                    var href = rawmenu.LinkElement.getAttribute("href");
                    if (!string.IsNullOrEmpty(href))
                    {
                        if (HasLink(menu, ref href))
                        {
                            return true;
                        }
                    }
                }
                foreach (var item in rawmenu.Children)
                {
                    if (GuessSameMenu(menu, item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasLink(Menu menu, ref string href)
        {
            if (!string.IsNullOrEmpty(menu.Url))
            {
                if (menu.Url == href)
                {
                    return true;
                }
            }

            foreach (var item in menu.children)
            {
                if (HasLink(item, ref href))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetMenuName(SiteDb siteDb, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "Menu";
            }
            if (siteDb.Menus.IsNameAvailable(name))
            {
                return name;
            }

            for (int i = 0; i < 999; i++)
            {
                string newname = name + i.ToString();
                if (siteDb.Menus.IsNameAvailable(newname))
                {
                    return newname;
                }
            }
            return null;
        }

        public string DetectKoobooId(Document dom, Menu menu)
        {
            if (menu.children.Count == 0)
            {
                return null;
            }

            if (menu.children.Count > 1)
            {
                var links = menu.children.Select(o => o.Url).ToList();
                return DomService.DetectKoobooId(dom, links);
            }
            else
            {
                var onlychild = menu.children[0];
                var koobooid = DetectKoobooId(dom, onlychild);
                // upgrade to one that contains that koobooid.
                // TODO: to be implemented.
            }
            return null;
        }
    }
}