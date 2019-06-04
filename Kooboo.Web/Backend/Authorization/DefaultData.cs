using Kooboo.Sites.Authorization.Model;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization
{
    public static class WebDefaultData
    {

        private static object _locker = new object();

        private static PermissionViewModel _availableData;

        public static PermissionViewModel Available
        {
            get
            {
                if (_availableData == null)
                {
                    lock (_locker)
                    {
                        if (_availableData == null)
                        {

                            _availableData = new PermissionViewModel();

                            PermissionViewModel system = new PermissionViewModel() { Name = "System" };
                            system.SubItems.Add(new PermissionViewModel() { Name = "Settings" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "TransferTask" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Config" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Text" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Domains" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Sync" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "SiteLogs" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "VisitorLogs" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Disk" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Jobs" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "SiteUser" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Roles" });
                            system.SubItems.Add(new PermissionViewModel() { Name = "Events" });

                            _availableData.SubItems.Add(system);

                            PermissionViewModel development = new PermissionViewModel() { Name = "Development" };
                            development.SubItems.Add(new PermissionViewModel() { Name = "Layouts" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Views" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Forms" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Menus" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Scripts" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Styles" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Code" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Urls" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "Search" });
                            development.SubItems.Add(new PermissionViewModel() { Name = "DataSource" });

                            _availableData.SubItems.Add(development);

                            PermissionViewModel contents = new PermissionViewModel() { Name = "Contents" };
                            contents.SubItems.Add(new PermissionViewModel() { Name = "Contents" });
                            contents.SubItems.Add(new PermissionViewModel() { Name = "ContentTypes" });
                            contents.SubItems.Add(new PermissionViewModel() { Name = "Labels" });
                            contents.SubItems.Add(new PermissionViewModel() { Name = "HtmlBlocks" });
                            contents.SubItems.Add(new PermissionViewModel() { Name = "Files" });

                            _availableData.SubItems.Add(contents);

                            PermissionViewModel database = new PermissionViewModel() { Name = "Database" };
                            database.SubItems.Add(new PermissionViewModel() { Name = "Table" });
                            database.SubItems.Add(new PermissionViewModel() { Name = "TableRelation" });
                            database.SubItems.Add(new PermissionViewModel() { Name = "Key-Value" });


                            _availableData.SubItems.Add(database);
                        }
                    }
                }

                return _availableData.Clone();
            }

        }


        public static PermissionViewModel GetDfaultAvailable()
        {
            PermissionViewModel result = new PermissionViewModel();

            var allmenus = Lib.IOC.Service.GetInstances<ICmsMenu>();

            foreach (var item in allmenus)
            {
                string permission = GetPermissionString(item); 
                if (!string.IsNullOrWhiteSpace(permission))
                {
                    AppendPermission(result, permission); 
                }
            }
            return result; 
        }

        public static void AppendPermission(PermissionViewModel result, string permission)
        {
            var sep = "/\\".ToCharArray();

            string[] parts = permission.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in parts)
            {
                var find = result.SubItems.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Name, item)); 
                if (find == null)
                {
                    find = new PermissionViewModel() { Name = item };
                    result.SubItems.Add(find);                  
                }
                result = find;
            }
        }
      

        public static string GetPermissionString(ICmsMenu menu)
        {
            if (menu == null)
            {
                return null; 
            }
            if (menu is ISideBarMenu)
            {
                var sidebarmenu = menu as ISideBarMenu; 
                if (sidebarmenu.Parent == SideBarSection.Root)
                {
                    return sidebarmenu.Name; 
                }
                else
                {
                    return sidebarmenu.Parent.ToString() + "/" + sidebarmenu.Name; 
                }
            }
            
            else if (menu is IFeatureMenu)
            {
                return "feature/" + menu.Name; 
            }

            return null; 
             
        }


        private static RolePermission _master;
        public static RolePermission Master
        {
            get
            {
                if (_master == null)
                {
                    _master = new RolePermission();
                    _master.Name = "master";
                    _master.Permission.Add("Development");
                    _master.Permission.Add("System");
                    _master.Permission.Add("Contents");
                    _master.Permission.Add("Database");
                }
                return _master;
            }
        }

        private static RolePermission _developer;
        public static RolePermission Developer
        {
            get
            {
                if (_developer == null)
                {
                    _developer = new RolePermission();
                    _developer.Name = "developer";
                    _developer.Permission.Add("Development");
                    _developer.Permission.Add("Contents");
                    _developer.Permission.Add("Database");
                }
                return _developer;
            }
        }

        private static RolePermission _contentmanager;
        public static RolePermission ContentManager
        {
            get
            {
                if (_contentmanager == null)
                {
                    _contentmanager = new RolePermission();
                    _contentmanager.Name = "contentmanager";
                    _contentmanager.Permission.Add("Contents");
                    _contentmanager.Permission.Add("Database");
                }
                return _contentmanager;
            }
        }

        public static RolePermission GetDefault(string name)
        {
            if (name == null)
            {
                return null;
            }

            name = name.ToLower();
            if (name == Master.Name)
            {
                return Master;
            }
            else if (name == Developer.Name)
            {
                return Developer;
            }
            else if (name == ContentManager.Name)
            {
                return ContentManager;
            }

            return null;

        }

        public static RolePermission GetDefault(Guid id)
        {
            if (id == Master.Id)
            {
                return Master;
            }
            else if (id == Developer.Id)
            {
                return Developer;
            }
            else if (id == ContentManager.Id)
            {
                return ContentManager;
            }
            return null;
        }

    }
}
