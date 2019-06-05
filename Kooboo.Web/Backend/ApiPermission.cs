using Kooboo.Data.Context;
using Kooboo.Sites.Authorization;
using Kooboo.Web.Authorization;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Backend
{
    public static class ApiPermission
    {
        private static Type PermissionLinkType { get; set; } = typeof(IApiPermissionLink<>);

        public static bool IsAllow(RenderContext context, Kooboo.Api.ApiMethod method)
        {
            if (context.User == null)
            {
                if (method.ClassInstance == null || !(method.ClassInstance is IApiPermissionString))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            if (context.User.IsAdmin)
            {
                return true;
            }
            else
            {

                string PermissionString = null;

                if (method.ClassInstance != null)
                {
                    if (method.ClassInstance is IPermissionControl)
                    {

                        if (method.ClassInstance is IApiPermissionString)
                        {
                            var permissionclass = method.ClassInstance as IApiPermissionString;
                            if (permissionclass != null)
                            {
                                PermissionString = permissionclass.Permission;
                            }
                        }
                        else
                        {
                            var instancetype = method.ClassInstance.GetType();

                            var type = GetPerminssionLinkUndertype(instancetype);

                            if (type != null)
                            {
                                var menuinstance = Activator.CreateInstance(type) as ICmsMenu;
                                if (menuinstance != null)
                                {
                                    PermissionString = MenuManager.GetPermissionString(menuinstance);
                                }
                            }

                        }
                    }
                }
                else
                {
                    //if (method.DeclareType != null)
                    //{
                    //    var instance = Activator.CreateInstance(method.DeclareType);
                    //    if (instance != null && instance is IApiPermissionString)
                    //    {
                    //        var permissionclass = instance as IApiPermissionString;
                    //        if (permissionclass != null)
                    //        {
                    //            PermissionString = permissionclass.Permission;
                    //        }
                    //    }
                    //}
                }


                if (string.IsNullOrWhiteSpace(PermissionString))
                {
                    return true;
                }
                else
                {
                    var role = SiteUserService.GetRolePermission(context);

                    if (role == null)
                    {
                        return false;
                    }
                    return Kooboo.Sites.Authorization.PermissionService.HasPermission(PermissionString, role.Tree);
                }

            }
        }


        public static Type GetPerminssionLinkUndertype(Type ApiClassType)
        {
            var interfaces = ApiClassType.GetInterfaces().Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == PermissionLinkType);

            if (interfaces.Any())
            {
                return interfaces.First().GetGenericArguments().First();

            }

            return null;
        }
    }

}
