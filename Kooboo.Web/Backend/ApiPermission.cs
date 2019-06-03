using Kooboo.Data.Context;
using Kooboo.Sites.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend
{
    public static class ApiPermission
    {

        public static bool IsAllow(RenderContext context, Kooboo.Api.ApiMethod method)
        {
            if (context.User == null)
            {
                if (method.ClassInstance == null || !(method.ClassInstance is IApiPermission))
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
                var role = SiteUserService.GetRolePermission(context);

                if (role == null)
                {
                    return false;
                }

                string PermissionString = null;

                if (method.ClassInstance != null)
                {
                    if (method.ClassInstance is IApiPermission)
                    {
                        var permissionclass = method.ClassInstance as IApiPermission;
                        if (permissionclass != null)
                        {
                            PermissionString = permissionclass.Permission;
                        }
                    }
                }
                else
                {
                    if (method.DeclareType != null)
                    {
                        var instance = Activator.CreateInstance(method.DeclareType);
                        if (instance != null && instance is IApiPermission)
                        {
                            var permissionclass = instance as IApiPermission;
                            if (permissionclass != null)
                            {
                                PermissionString = permissionclass.Permission;
                            }
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(PermissionString))
                {
                    return true;
                }
                else
                {
                    return Kooboo.Sites.Authorization.PermissionHelper.HasPermission(PermissionString, role.Tree);
                }

            }
        }

    }

}
