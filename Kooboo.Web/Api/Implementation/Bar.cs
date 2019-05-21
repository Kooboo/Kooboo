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

            var menus = MenuContainer.SubMenus(typeof(ITopMenu));

            foreach (var item in menus)
            {
                header.Menu.Add(new CmsMenuViewModel(item, call.Context));
            }

            //header.Menu.Add(new GlobalMenuItem { Name = Hardcoded.GetValue("Market", context), Url = "/_api/user/onlineserver", Icon = "fa fa-plug", Count = 0, BadgeIcon = "badge-primary", OpenInNewWindow = true });

            //header.Menu.Add(new GlobalMenuItem { Name = Hardcoded.GetValue("E-Commerce", context), Url = AdminUrl("Ecommerce"), Icon = "fa fa-shopping-cart", Count = 0, BadgeIcon = "badge-success" });

            header.User = new DisplayName() { name = user.UserName, id = user.UserName, Language = user.Language };

            return header;
        }

        public List<MenuItem> SiteMenu(ApiCall call)
        {
            var user = call.Context.User;

            if (call.WebSite == null)
            {
                return new List<MenuItem>();
            }

            List<MenuItem> menus = new List<MenuItem>();

            menus.Add(new MenuItem
            {
                Name = Hardcoded.GetValue("Feature", call.Context),
                Items = SiteMenu_Feature(call)
            });

            menus.Add(new MenuItem
            {
                Name = Hardcoded.GetValue("Advance", call.Context),
                Items = SiteMenu_Advance(call)
            });

            // additional menu for the extension. 
            var sitemenus = MenuContainer.SubMenus(typeof(ISideBarMenu));
            var oldmenues = sitemenus.Select(o => MenuHelper.ConvertToOld(o, call.Context)).ToList();

            if (oldmenues != null && oldmenues.Any())
            {
                menus.AddRange(oldmenues);
            }
             
            if (user.IsAdmin)
            {
                return menus;
            }
            else
            {
                var sitedb = call.Context.WebSite.SiteDb();
                var siteuser = sitedb.SiteUser.Get(call.Context.User.Id);

                if (siteuser == null)
                {
                    return null;
                }
                else
                {
                    CheckRights(menus, siteuser.Role);

                    return menus;
                }
            }

        }

        private void AppendImplementation(List<MenuItem> menus)
        {

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

        private List<MenuItem> SiteMenu_Feature(ApiCall call)
        {
            User user = call.Context.User;
            SiteDb siteDb = call.WebSite.SiteDb();
            var context = call.Context;

            return new List<MenuItem>
            {
                new MenuItem { Name = Hardcoded.GetValue("Media library", context), Icon="icon icon-picture", Url = AdminUrl("Contents/Images", siteDb) },
                new MenuItem { Name = Hardcoded.GetValue("Pages", context), Icon = "icon icon-layers", Url = AdminUrl("Pages", siteDb) },
                new MenuItem{ Name = Hardcoded.GetValue("Diagnosis", context), Icon = "icon icon-support", Url = AdminUrl("Sites/Diagnosis", siteDb) },
            };
        }

        protected virtual List<MenuItem> SiteMenu_Advance(ApiCall call)
        {
            User user = call.Context.User;
            SiteDb siteDb = call.Context.WebSite.SiteDb();
            var context = call.Context;

            var items = new List<MenuItem>();

            var sysmenu = new MenuItem
            {
                Name = Hardcoded.GetValue("System", context),
                Icon = "icon icon-settings",
                Items =
                {
                    new MenuItem{ Name = Hardcoded.GetValue("Settings",context), Url = AdminUrl("System/Settings", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Settings },
                    new MenuItem{ Name = Hardcoded.GetValue("TransferTask",context), Url = AdminUrl("System/TransferTask", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Settings },
                    new MenuItem{ Name = Hardcoded.GetValue("Config",context), Url = AdminUrl("System/CoreSettings", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Settings },
                    new MenuItem{ Name = Hardcoded.GetValue("Text",context), Url = AdminUrl("System/KConfig", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Configs },
                    new MenuItem{ Name = Hardcoded.GetValue("Domains", context), Url = AdminUrl("System/Domains", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Domains},
                    new MenuItem{ Name = Hardcoded.GetValue("Sync", context),  Url = AdminUrl("Sync", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Synchronization},
                    new MenuItem{ Name = Hardcoded.GetValue("SiteLogs",context),Url = AdminUrl("System/SiteLogs", siteDb), ActionRights = Sites.Authorization.Actions.Systems.SiteLogs },
                    new MenuItem{ Name = Hardcoded.GetValue("VisitorLogs",context),Url = AdminUrl("System/VisitorLogs", siteDb),ActionRights = Sites.Authorization.Actions.Systems.VisitorLogs },
                    new MenuItem{ Name = Hardcoded.GetValue("Disk", context),Url = AdminUrl("System/Disk", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Disk },
                    new MenuItem{ Name = Hardcoded.GetValue("Jobs", context), Url = AdminUrl("System/Jobs", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Jobs },
                    new MenuItem{ Name = Hardcoded.GetValue("SiteUser", context), Url = AdminUrl("System/SiteUser", siteDb), ActionRights = Sites.Authorization.Actions.Systems.SiteUser },

                }
            };

            if (call.WebSite != null && call.WebSite.EnableFrontEvents)
            {
                var eventmenus = SiteMenu_Events(call);
                if (eventmenus != null && eventmenus.Items.Count() > 0)
                {
                    sysmenu.Items.Add(eventmenus);
                }
            }

            items.Add(sysmenu);



            items.Add(new MenuItem
            {
                Name = Hardcoded.GetValue("Development", context),
                Icon = "icon fa fa-code",
                Items =
                {
                    new MenuItem { Name = Hardcoded.GetValue("Layouts", context), Url = AdminUrl("Development/Layouts", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Layouts },
                    new MenuItem { Name = Hardcoded.GetValue("Views", context), Url = AdminUrl("Development/Views", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Views },
                    new MenuItem { Name = Hardcoded.GetValue("Forms", context), Url = AdminUrl("Development/Forms", siteDb),  ActionRights = Sites.Authorization.Actions.Developments.Forms },
                    new MenuItem { Name = Hardcoded.GetValue("Menus", context),  Url = AdminUrl("Development/Menus", siteDb),  ActionRights = Sites.Authorization.Actions.Developments.Menus },
                    new MenuItem { Name = Hardcoded.GetValue("Scripts", context), Url = AdminUrl("Development/Scripts", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Scripts },
                    new MenuItem { Name = Hardcoded.GetValue("Styles", context), Url = AdminUrl("Development/Styles", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Styles },
                    new MenuItem { Name = Hardcoded.GetValue("Code",context), Url = AdminUrl("Development/Code", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Code },
                    new MenuItem { Name = Hardcoded.GetValue("Urls",context), Url = AdminUrl("Development/URLs", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Urls },
                    new MenuItem { Name = Hardcoded.GetValue("Search", context),  Url = AdminUrl("Development/Search", siteDb) , ActionRights = Sites.Authorization.Actions.Developments.Search},
                    new MenuItem { Name = Hardcoded.GetValue("DataSource",context), Url = AdminUrl("Development/DataSources", siteDb), ActionRights = Sites.Authorization.Actions.Developments.DataSource },
                }
            });

            items.Add(new MenuItem
            {
                Name = Hardcoded.GetValue("Contents", context),
                Icon = "icon fa fa-files-o",
                Items =
                {
                    new MenuItem
                    {
                        Name = Hardcoded.GetValue("Contents", context),
                        Url = AdminUrl("Contents/TextContents", siteDb),
                        Items = SiteMenu_SubContent(user, siteDb),  ActionRights = Sites.Authorization.Actions.Contents.Content
                    },
                    new MenuItem { Name = Hardcoded.GetValue("ContentTypes", context), Url = AdminUrl("Contents/ContentTypes", siteDb) ,  ActionRights = Sites.Authorization.Actions.Contents.ContentTypes},
                    new MenuItem { Name = Hardcoded.GetValue("Labels", context), Url = AdminUrl("Contents/Labels", siteDb),  ActionRights = Sites.Authorization.Actions.Contents.Labels },
                    new MenuItem { Name = Hardcoded.GetValue("HtmlBlocks", context), Url = AdminUrl("Contents/HtmlBlocks", siteDb) ,  ActionRights = Sites.Authorization.Actions.Contents.HtmlBlocks},
                    new MenuItem { Name = Hardcoded.GetValue("Files", context), Url = AdminUrl("Storage/Files", siteDb) },
                }
            });

            //items.Add(new MenuItem
            //{
            //    Name = Hardcoded.GetValue("E-Commerce", context),
            //    Icon = "icon fa fa-shopping-cart",
            //    Items =
            //    {
            //        new MenuItem { Name = Hardcoded.GetValue("Products management", context), Url = AdminUrl("ECommerce/Products", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Product types", context), Url = AdminUrl("ECommerce/Product/Types", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Product categories", context), Url = AdminUrl("ECommerce/Product/Categories", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Customers", context), Url = AdminUrl("ECommerce/Product/Categories1", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Orders", context), Url = AdminUrl("ECommerce/Product/Categories2", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Marketing", context), Url = AdminUrl("ECommerce/Product/Categories3", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Reports", context), Url = AdminUrl("ECommerce/Product/Categories4", siteDb) },
            //        new MenuItem { Name = Hardcoded.GetValue("Settings", context), Url = AdminUrl("ECommerce/Product/Categories5", siteDb) }                    
            //    }
            //});

            items.Add(new MenuItem
            {
                Name = Hardcoded.GetValue("Database", context),
                Icon = "icon fa fa-database",
                Items =
                {
                    new MenuItem { Name = Hardcoded.GetValue("Table",context), Url = AdminUrl("Storage/Database", siteDb) },
                    new MenuItem { Name = Hardcoded.GetValue("Table Relation",context), Url = AdminUrl("Storage/TableRelation", siteDb) },
                    new MenuItem { Name = Hardcoded.GetValue("Key-Value",context), Url = AdminUrl("Storage/KeyValue", siteDb) }
                }
            });

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

        protected virtual MenuItem SiteMenu_Events(ApiCall call)
        {
            User user = call.Context.User;
            SiteDb siteDb = call.WebSite.SiteDb();
            var context = call.Context;

            var root = new MenuItem
            {
                Name = Hardcoded.GetValue("Events", context),
                Items =
                {
                    new MenuItem { Name = Hardcoded.GetValue("Overview", context), Url = AdminUrl("Events", siteDb) }
                },
                ActionRights = Kooboo.Sites.Authorization.Actions.Systems.Events
            };

            var names = Enum.GetNames(typeof(Kooboo.Sites.FrontEvent.enumEventType));

            List<GroupEvent> groupnames = new List<GroupEvent>();
            foreach (var item in names)
            {
                GroupEvent eventname = new GroupEvent();
                eventname.name = item;
                eventname.group = GetEventGroup(item);
                groupnames.Add(eventname);
            }


            foreach (var group in groupnames.GroupBy(o => o.group))
            {
                var item = new MenuItem { Name = group.Key };

                foreach (var oneevent in group.ToList())
                {
                    item.Items.Add(new MenuItem
                    {
                        Name = oneevent.name,
                        Url = AdminUrl("Events/Event?name=" + oneevent.name.ToString(), siteDb)
                    });
                }

                root.Items.Add(item);
            }

            return root;
        }

        public class GroupEvent
        {
            public string group { get; set; }
            public string name { get; set; }
        }

        private string GetEventGroup(string input)
        {
            string group = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                var currentchar = input[i];
                if (i == 0)
                {
                    group += currentchar;
                }
                else
                {
                    if (Lib.Helper.CharHelper.isUppercaseAscii(currentchar))
                    {
                        return group;
                    }
                    else
                    {
                        group += currentchar;
                    }
                }
            }
            return group;
        }

        private List<MenuItem> SiteMenu_SubContent(User user, SiteDb siteDb)
        {
            if (siteDb == null)
                return new List<MenuItem>();
            var folders = siteDb.ContentFolders.All();
            return SiteMenu_SubContent(user, siteDb, folders, Guid.Empty);
        }

        private List<MenuItem> SiteMenu_SubContent(User user, SiteDb siteDb, List<Kooboo.Sites.Contents.Models.ContentFolder> folders, Guid parentId)
        {
            return folders.Where(o => o.ParentFolderId == parentId).Select(o => new MenuItem
            {
                Name = o.Id.ToString(),
                DisplayName = o.DisplayName,
                Url = AdminUrl("Contents/TextContentsByFolder?folder=" + o.Id, siteDb),
                Items = SiteMenu_SubContent(user, siteDb, folders, o.Id)
            }).ToList();
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
