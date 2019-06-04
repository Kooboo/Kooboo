//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Models;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;
using Kooboo.Data.Context;

namespace Kooboo.Web.Api.Implementation
{
    public class DisplayName
    {
        public string name { get; set; }

        public string id { get; set; }

        public string Language { get; set; }
    }

    public class BarApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Bar";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public virtual HeaderMenu Header(ApiCall call)
        {
            var user = call.Context.User;
            HeaderMenu header = new HeaderMenu();

            var menus = MenuContainer.SubMenus(typeof(IHeaderMenu));

            foreach (var item in menus)
            {
                header.Menu.Add(new CmsMenuViewModel(item, call.Context));
            }

            //header.Menu.Add(new GlobalMenuItem { Name = Hardcoded.GetValue("Market", context), Url = "/_api/user/onlineserver", Icon = "fa fa-plug", Count = 0, BadgeIcon = "badge-primary", OpenInNewWindow = true }); 
            //header.Menu.Add(new GlobalMenuItem { Name = Hardcoded.GetValue("E-Commerce", context), Url = AdminUrl("Ecommerce"), Icon = "fa fa-shopping-cart", Count = 0, BadgeIcon = "badge-success" });

            header.User = new DisplayName() { name = user.UserName, id = user.UserName, Language = user.Language };

            return header;
        }

        public List<CmsMenuViewModel> SiteMenu(ApiCall call)
        {
            var user = call.Context.User;

            if (call.WebSite == null)
            {
                return new List<CmsMenuViewModel>();
            }

            List<CmsMenuViewModel> menus = new List<CmsMenuViewModel>();

            var feature = new CmsMenuViewModel(Hardcoded.GetValue("Feature", call.Context));

            feature.Items = MenuContainer.FeatureMenus.Select(o => new CmsMenuViewModel(o, call.Context)).ToList();

            MenuManager.VerifySortSideBar(feature.Items, call.Context); 

            menus.Add(feature); 

            menus.Add(SiteBarAdvancedMenu(call));

            // additional menu for the extension. 
            //var sitemenus = MenuContainer.SubMenus(typeof(ISideBarMenu));

            //var oldmenues = sitemenus.Select(o => MenuHelper.ConvertToOld(o, call.Context)).ToList();

            //if (oldmenues != null && oldmenues.Any())
            //{
            //    menus.AddRange(oldmenues);
            //}

            //if (user.IsAdmin)
            //{
            //    return menus;
            //}
            //else
            //{
            //    var sitedb = call.Context.WebSite.SiteDb();
            //    var siteuser = sitedb.SiteUser.Get(call.Context.User.Id);

            //    if (siteuser == null)
            //    {
            //        return null;
            //    }
            //    else
            //    {
            //        CheckRights(menus, siteuser.Role);

            //        return menus;
            //    }
            //}  
            //TODO: check user rights...
            return menus;
        }


        private CmsMenuViewModel SiteBarAdvancedMenu(ApiCall call)
        {
            var context = call.Context;
            var advanceheadline = Hardcoded.GetValue("Advance", context); 
            var advance = new CmsMenuViewModel(advanceheadline, advanceheadline);

            //Hardcoded.GetValue("System", context)
            //Hardcoded.GetValue("Development", context)
            //Hardcoded.GetValue("Contents", context)
            //Hardcoded.GetValue("Database", context)

            var system = new CmsMenuViewModel(SideBarSection.System.ToString(), Hardcoded.GetValue("System", context)) { Icon = "icon icon-settings" };
            var development = new CmsMenuViewModel(Hardcoded.GetValue(SideBarSection.Development.ToString(), context)) { Icon = "icon fa fa-code" };
            var content = new CmsMenuViewModel(Hardcoded.GetValue(SideBarSection.Contents.ToString(), context)) { Icon = "icon fa fa-files-o" };
            var database = new CmsMenuViewModel(Hardcoded.GetValue(SideBarSection.Database.ToString(), context)) { Icon = "icon fa fa-database" };

            advance.Items.Add(system);
            advance.Items.Add(development);
            advance.Items.Add(content);
            advance.Items.Add(database);

            var sitebarmenus = MenuContainer.SideBarMenus;

            foreach (var item in sitebarmenus)
            {
                if (item.Parent == SideBarSection.Root)
                {
                    advance.Items.Add(new CmsMenuViewModel(item, context));
                }
                else if (item.Parent == SideBarSection.System)
                {
                    system.Items.Add(new CmsMenuViewModel(item, context));
                }
                else if (item.Parent == SideBarSection.Development)
                {
                    development.Items.Add(new CmsMenuViewModel(item, context));
                }
                else if (item.Parent == SideBarSection.Contents)
                {
                    content.Items.Add(new CmsMenuViewModel(item, context));
                }
                else if (item.Parent == SideBarSection.Database)
                {
                    database.Items.Add(new CmsMenuViewModel(item, context));
                }
            }
             
            MenuManager.VerifySortSideBar(advance.Items, call.Context); 

            return advance; 

        }




        private void CheckRights(List<MenuItem> menus, Sites.Authorization.EnumUserRole role)
        {
            foreach (var item in menus)
            {
                RemoveUnAccessSub(item, role);
            }

            // clean unused menu item... 
            foreach (var item in menus)
            {
                CleanEmpty(item);
            }
        }

        public void CleanEmpty(MenuItem Menu)
        {
            foreach (var item in Menu.Items)
            {
                if (string.IsNullOrEmpty(item.Url) && (item.Items != null && item.Items.Count() > 0))
                {
                    CleanEmpty(item);
                }
            }

            List<int> toremove = new List<int>();

            int len = Menu.Items.Count();

            for (int i = 0; i < len; i++)
            {
                var item = Menu.Items[i];

                if (string.IsNullOrWhiteSpace(item.Url) && (item.Items == null || item.Items.Count() == 0))
                {
                    toremove.Add(i);
                }
            }

            foreach (var item in toremove.OrderByDescending(o => o))
            {
                Menu.Items.RemoveAt(item);
            }
        }

        public void RemoveUnAccessSub(MenuItem Menu, Kooboo.Sites.Authorization.EnumUserRole userRole)
        {
            List<int> toremove = new List<int>();

            int len = Menu.Items.Count();

            for (int i = 0; i < len; i++)
            {
                var item = Menu.Items[i];

                if (item.ActionRights != 0)
                {
                    if (!Sites.Authorization.RoleManagement.HasRights(item.ActionRights, userRole))
                    {
                        toremove.Add(i);
                    }
                }
                else if (string.IsNullOrEmpty(item.Url) && (item.Items != null && item.Items.Count > 0))
                {
                    // check the sub items. 
                    RemoveUnAccessSub(item, userRole);
                }
            }

            foreach (var item in toremove.OrderByDescending(o => o))
            {
                Menu.Items.RemoveAt(item);
            }

        }

        public List<MenuItem> EmailMenu(ApiCall call)
        {
            var context = call.Context;
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(context.User.CurrentOrgId);
            var addresses = orgdb.EmailAddress.ByUser(context.User.Id);

            return new MenuItem[]
            {
                new MenuItem { Name = Hardcoded.GetValue("Inbox", context), Icon = "icon glyphicon glyphicon-inbox", Url = AdminUrl("emails/inbox"), Items = EmailMenu_Addresses("Inbox", addresses) },
                new MenuItem { Name = Hardcoded.GetValue("Sent", context), Icon = "icon fa fa-send", Url = AdminUrl("emails/sent"), Items = EmailMenu_Addresses("Sent", addresses) },
                new MenuItem { Name = Hardcoded.GetValue("Draft", context), Icon = "icon fa fa-edit", Url = AdminUrl("emails/draft"), Items = EmailMenu_Addresses("Draft", addresses) },
                new MenuItem { Name = Hardcoded.GetValue("Trash", context), Icon = "icon fa fa-trash", Url = AdminUrl("emails/trash"), Items = EmailMenu_Addresses("Trash", addresses) },
                new MenuItem { Name =Hardcoded.GetValue("Spam", context), Icon = "icon fa fa-recycle", Url = AdminUrl("emails/spam"), Items = EmailMenu_Addresses("Spam", addresses)},
                new MenuItem(),
                new MenuItem { Name = Hardcoded.GetValue("Address", context), Icon = "icon fa fa-at", Url = AdminUrl("emails/address") }
            }.ToList();
        }

        public virtual List<MenuItem> DomainMenu(ApiCall call)
        {
            var menus = new MenuItem[]
             {
                new MenuItem { Name =Hardcoded.GetValue("Domains", call.Context), Icon = "icon fa fa-at", Url = AdminUrl("Domains") },
                new MenuItem { Name = Hardcoded.GetValue("SiteBindings", call.Context), Icon = "icon fa fa-link", Url = AdminUrl("Domains/SiteBindings") },
                new MenuItem { Name = Hardcoded.GetValue("SiteMirror", call.Context), Icon = "icon fa fa-sitemap", Url = AdminUrl("Domains/SiteMirror") },

             }.ToList();

            return menus;
        }


        protected virtual List<MenuItem> SiteMenu_Advance(ApiCall call)
        {
            User user = call.Context.User;
            SiteDb siteDb = call.Context.WebSite.SiteDb();
            var context = call.Context;

            var items = new List<MenuItem>();
             

            if (siteDb.WebSite.EnableMultilingual)
            {
                List<string> othercultures = new List<string>();
                foreach (var item in siteDb.WebSite.Culture.Keys.ToList())
                {
                    if (item.ToLower() != siteDb.WebSite.DefaultCulture.ToLower())
                    {
                        othercultures.Add(item);
                    }
                }

                items.Add(new MenuItem
                {
                    Name = Hardcoded.GetValue("Multilingual", context),
                    Icon = "icon glyphicon glyphicon-globe",
                    ActionRights = Sites.Authorization.Actions.Contents.Multilingual,
                    Items = othercultures.Select(it => new MenuItem
                    {
                        Name = it,
                        Items = SiteMenu_MultiLanguages(call, it)
                    }).ToList()
                });
            }

            return items;
        } 
        
        private List<MenuItem> SiteMenu_MultiLanguages(ApiCall call, string lang)
        {
            User user = call.Context.User;
            SiteDb siteDb = call.WebSite.SiteDb();
            var context = call.Context;

            return new List<MenuItem>
            {
                new MenuItem
                {
                    Name = Hardcoded.GetValue("Contents", context),
                    Items = SiteMenu_SubTranslateContent(user, siteDb, lang),
                },
                new MenuItem
                {
                    Name = Hardcoded.GetValue("Labels",context),
                    Url = AdminUrl("Multilingual/Labels?lang=" + lang, siteDb)
                },
                new MenuItem
                {
                    Name = Hardcoded.GetValue("HtmlBlocks", context),
                    Url = AdminUrl("Multilingual/HtmlBlocks?Lang=" + lang, siteDb),
                }
            };
        }

        private List<MenuItem> SiteMenu_SubTranslateContent(User user, SiteDb siteDb, string lang)
        {
            var folders = siteDb.ContentFolders.All();
            return SiteMenu_SubTranslateContent(user, siteDb, lang, folders, Guid.Empty);
        }

        private List<MenuItem> SiteMenu_SubTranslateContent(User user, SiteDb siteDb, string lang, List<Kooboo.Sites.Contents.Models.ContentFolder> folders, Guid parentId)
        {
            return folders.Where(o => o.ParentFolderId == parentId).Select(item => new MenuItem
            {
                Name = item.Id.ToString(),
                DisplayName = item.DisplayName,
                Url = AdminUrl(String.Format("Multilingual/TextContentsByFolder?folder={1}&lang={0}", lang, item.Id), siteDb),
                Items = SiteMenu_SubTranslateContent(user, siteDb, lang, folders, item.Id)
            }).ToList();
        }

        private List<MenuItem> EmailMenu_Addresses(string folderName, IEnumerable<Kooboo.Mail.EmailAddress> addresses)
        {
            var lowerFolderName = folderName.ToLower();
            return addresses.Select(o => new MenuItem
            {
                Name = o.Address,
                Url = AdminUrl($"emails/{lowerFolderName}?address={o.Address}")
            }).ToList();
        }

        protected string AdminUrl(string relativeUrl)
        {
            return "/_Admin/" + relativeUrl;
        }

        private string AdminUrl(string relativeUrl, SiteDb siteDb)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            if (siteDb != null)
            {
                para.Add("SiteId", siteDb.Id.ToString());
            }
            return Kooboo.Lib.Helper.UrlHelper.AppendQueryString("/_Admin/" + relativeUrl, para);
        }
    }


    public class HeaderMenu
    {
        public List<CmsMenuViewModel> Menu { get; set; } = new List<CmsMenuViewModel>();

        public DisplayName User { get; set; }

        public EmailCount Email { get; set; } = new EmailCount();

        public class EmailCount
        {
            public int Count { get; set; }
        }
    }
}
