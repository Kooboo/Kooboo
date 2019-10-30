//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Authorization
{
    public static class RoleManagement
    {
        private static Dictionary<EnumUserRole, HashSet<uint>> RoleActions { get; set; }

        static RoleManagement()
        {
            // this is hard code for now..

            if (RoleActions == null)
            {
                RoleActions = new Dictionary<EnumUserRole, HashSet<uint>>();

                HashSet<uint> admin = new HashSet<uint> {Actions.Admin};

                RoleActions.Add(EnumUserRole.Administrator, admin);

                HashSet<uint> siteowner = new HashSet<uint>
                {
                    Actions.System, Actions.Development, Actions.Content, Actions.Storage
                };

                RoleActions.Add(EnumUserRole.SiteMaster, admin);

                HashSet<uint> development = new HashSet<uint>
                {
                    Actions.Development,
                    Actions.Content,
                    Actions.Storage,
                    Actions.Systems.SiteLogs,
                    Actions.Systems.Jobs,
                    Actions.Systems.Disk,
                    Actions.Systems.Events
                };


                RoleActions.Add(EnumUserRole.Developer, development);

                HashSet<uint> contentManager = new HashSet<uint> {Actions.Content, Actions.Storage};


                RoleActions.Add(EnumUserRole.ContentManager, contentManager);
            }
        }

        public static bool HasRights(uint actionRights, EnumUserRole role)
        {
            if (RoleActions.ContainsKey(role))
            {
                var items = RoleActions[role];

                if (Hierarchy.HasRights(actionRights, items))
                {
                    return true;
                }
            }

            return false;
        }
    }
}