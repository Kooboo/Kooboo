using Kooboo.Data.Context;
using Kooboo.Sites.Authorization.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization
{
  public static  class ApiPermission
    {
         
        public static bool IsAllow(string permissiontree, RenderContext context)
        {
            // get user permission tree and check... 
            return true; 
        }


        // The original generated list...
        public static PermissionViewModel MasterTemplate()
        {
            PermissionViewModel root = new PermissionViewModel(); 

            PermissionViewModel system = new PermissionViewModel() { Name = "systems" };
            system.SubItems.Add(new PermissionViewModel() { Name = "Settings" });
            system.SubItems.Add(new PermissionViewModel() { Name = "Domains" });
            system.SubItems.Add(new PermissionViewModel() { Name = "SiteLogs" });
            system.SubItems.Add(new PermissionViewModel() { Name = "VisitorLogs" });
            system.SubItems.Add(new PermissionViewModel() { Name = "Disk" });
            system.SubItems.Add(new PermissionViewModel() { Name = "Jobs" });
            system.SubItems.Add(new PermissionViewModel() { Name = "SiteUser" });
            system.SubItems.Add(new PermissionViewModel() { Name = "Events" });
            system.SubItems.Add(new PermissionViewModel() { Name = "Synchronization" });
            system.SubItems.Add(new PermissionViewModel() { Name = "Configs" });

            root.SubItems.Add(system); 

            PermissionViewModel development = new PermissionViewModel() { Name = "Developments" };
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

            root.SubItems.Add(development);

            return root;
        } 

        //public static uint Content = Hierarchy.GetInt(1, 3);

        //public static class Contents
        //{
        //    public static uint Content = Hierarchy.GetInt(1, 3, 1);
        //    public static uint Labels = Hierarchy.GetInt(1, 3, 2);
        //    public static uint HtmlBlocks = Hierarchy.GetInt(1, 3, 3);
        //    public static uint ContentTypes = Hierarchy.GetInt(1, 3, 4);

        //    public static uint Multilingual = Hierarchy.GetInt(1, 3, 5);

        //}

        //public static uint Storage = Hierarchy.GetInt(1, 4);

        //public static class Storages
        //{
        //    public static uint Database = Hierarchy.GetInt(1, 4, 1);
        //    public static uint KeyValue = Hierarchy.GetInt(1, 4, 2);
        //    public static uint Files = Hierarchy.GetInt(1, 4, 3);
        //}





    }
}
