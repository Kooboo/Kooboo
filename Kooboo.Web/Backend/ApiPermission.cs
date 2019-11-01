using Kooboo.Data.Context;
using Kooboo.Web.Authorization;
using Kooboo.Web.Menus;
using System;
using System.Linq;

namespace Kooboo.Web.Backend
{
    public static class ApiPermission
    {
        private static Type PermissionLinkType { get; set; } = typeof(IApiPermissionLink<>);

        public static bool IsAllow(RenderContext context, Kooboo.Api.ApiMethod method)
        {
            if (context.User == null)
            {
                return method.ClassInstance == null || !(method.ClassInstance is IApiPermissionString);
            }

            if (context.User.IsAdmin)
            {
                return true;
            }
            else
            {
                string permissionString = null;

                if (method.ClassInstance is IPermissionControl)
                {
                    if (method.ClassInstance is IApiPermissionString permissionclass)
                    {
                        if (permissionclass != null)
                        {
                            permissionString = permissionclass.Permission;
                        }
                    }
                    else
                    {
                        var instancetype = method.ClassInstance.GetType();

                        var type = GetPerminssionLinkUndertype(instancetype);

                        if (type != null)
                        {
                            if (Activator.CreateInstance(type) is ICmsMenu menuinstance)
                            {
                                permissionString = MenuManager.GetPermissionString(menuinstance);
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(permissionString))
                {
                    return true;
                }
                else
                {
                    var role = SiteUserService.GetRolePermission(context);

                    return role != null && Kooboo.Sites.Authorization.PermissionService.HasPermission(permissionString, role.Tree);
                }
            }
        }

        public static Type GetPerminssionLinkUndertype(Type apiClassType)
        {
            var interfaces = apiClassType.GetInterfaces().Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == PermissionLinkType);

            return interfaces.Any() ? interfaces.First().GetGenericArguments().First() : null;
        }
    }
}