//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Authorization
{
   public static  class Actions
    {

        public static uint Admin = Hierarchy.GetInt(1);

        public static uint System = Hierarchy.GetInt(1, 1); 

        public static class Systems
        {
            public static uint Settings = Hierarchy.GetInt(1, 1, 1);

            public static uint Domains = Hierarchy.GetInt(1, 1, 2);

            public static uint SiteLogs = Hierarchy.GetInt(1, 1, 3);

            public static uint VisitorLogs =  Hierarchy.GetInt(1, 1, 4);

            public static uint Disk = Hierarchy.GetInt(1, 1, 5);

            public static uint Jobs = Hierarchy.GetInt(1, 1, 6);

            public static uint SiteUser = Hierarchy.GetInt(1, 1, 7);

            public static uint Events = Hierarchy.GetInt(1, 1, 8);

            public static uint Synchronization = Hierarchy.GetInt(1, 1, 9);

            public static uint Configs = Hierarchy.GetInt(1, 1, 10);
        }
         
        public static uint Development = Hierarchy.GetInt(1, 2);

        public static class Developments
        { 
            public static uint Layouts = Hierarchy.GetInt(1, 2, 1);
            public static uint Views = Hierarchy.GetInt(1, 2, 1);
            public static uint Forms = Hierarchy.GetInt(1, 2, 1);
            public static uint Menus = Hierarchy.GetInt(1, 2, 1);
            public static uint Scripts = Hierarchy.GetInt(1, 2, 1);

            public static uint Styles = Hierarchy.GetInt(1, 2, 1);
            public static uint Code = Hierarchy.GetInt(1, 2, 1);
            public static uint Urls = Hierarchy.GetInt(1, 2, 1);
            public static uint Search = Hierarchy.GetInt(1, 2, 1);

            public static uint DataSource = Hierarchy.GetInt(1, 2, 1);

        }

        public static uint Content = Hierarchy.GetInt(1, 3);

        public static class Contents
        {
            public static uint Content = Hierarchy.GetInt(1, 3, 1);
            public static uint Labels = Hierarchy.GetInt(1, 3, 2);
            public static uint HtmlBlocks = Hierarchy.GetInt(1, 3, 3);
            public static uint ContentTypes = Hierarchy.GetInt(1, 3, 4);

            public static uint Multilingual = Hierarchy.GetInt(1, 3, 5);

        }
         
        public static uint Storage = Hierarchy.GetInt(1, 4);

        public static class Storages
        {
            public static uint Database = Hierarchy.GetInt(1, 4, 1);
            public static uint KeyValue = Hierarchy.GetInt(1, 4, 2);
            public static uint Files = Hierarchy.GetInt(1, 4, 3);
        } 
    }
}
