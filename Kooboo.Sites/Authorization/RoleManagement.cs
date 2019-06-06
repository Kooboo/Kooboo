//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
 

namespace Kooboo.Sites.Authorization
{
  public static  class RoleManagement
    {
        private static Dictionary<EnumUserRole, HashSet<uint>> roleActions { get; set; }

        static RoleManagement()
        { 
            // this is hard code for now.. 
           
            if (roleActions == null)
            {
                roleActions = new Dictionary<EnumUserRole, HashSet<uint>>();

           
                HashSet<uint> admin = new HashSet<uint>();
                admin.Add(Actions.Admin);

                roleActions.Add(EnumUserRole.Administrator, admin);

                HashSet<uint> siteowner = new HashSet<uint>();
                siteowner.Add(Actions.System);
                siteowner.Add(Actions.Development);
                siteowner.Add(Actions.Content);
                siteowner.Add(Actions.Storage);

                roleActions.Add(EnumUserRole.SiteMaster, admin);


                HashSet<uint> development = new HashSet<uint>();

                development.Add(Actions.Development);
                development.Add(Actions.Content);
                development.Add(Actions.Storage);
                development.Add(Actions.Systems.SiteLogs); 
                development.Add(Actions.Systems.Jobs);
                development.Add(Actions.Systems.Disk);
                development.Add(Actions.Systems.Events); 

                roleActions.Add(EnumUserRole.Developer, development);
                 
                HashSet<uint> contentManager = new HashSet<uint>();
                 
                contentManager.Add(Actions.Content);
                contentManager.Add(Actions.Storage);

                roleActions.Add(EnumUserRole.ContentManager, contentManager); 

            }

        }
         
        public static bool HasRights(uint actionRights, EnumUserRole role)
        {
            if (roleActions.ContainsKey(role))
            {
                var items = roleActions[role]; 

                if (Hierarchy.HasRights(actionRights, items))
                {
                    return true; 
                }
            }

            return false; 
        }
  
    }
}
