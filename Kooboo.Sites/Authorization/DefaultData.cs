using Kooboo.Sites.Authorization.Model;
using Kooboo.Web.Menus;
using System;
 

namespace Kooboo.Sites.Authorization
{
    public static class DefaultData
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
                            _availableData = GetDefaultAvailable(); 
                        }
                    }
                }

                return _availableData.Clone();
            } 
        }


        public static PermissionViewModel GetDefaultAvailable()
        {
            PermissionViewModel result = new PermissionViewModel();

            var allmenus = Lib.IOC.Service.GetInstances<ICmsMenu>();

            foreach (var item in allmenus)
            {
                string permission = MenuService.GetPermissionString(item);
                if (!string.IsNullOrWhiteSpace(permission))
                {
                    AppendPermission(result, permission);
                }
            }
            return result;
        }

        private static void AppendPermission(PermissionViewModel result, string permission)
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
