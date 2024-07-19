using System.Linq;
using JWT;
using JWT.Serializers;
using Kooboo.Data.Context;
using Kooboo.Data.Context.UserProviders;
using Kooboo.Data.Definition;
using Kooboo.Data.Permission;
using Kooboo.Sites.Authorization;
using Kooboo.Sites.Authorization.Model;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Permission;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Api
{
    public static class PermissionService
    {
        static PermissionService()
        {
            EmbeddedRoles = new()
            {
                Master,
                Developer,
                ContentManager
            };
        }

        public static List<PermissionItem> GetList()
        {

            var apis = SiteApiProvider.GetAllDefinedApi();

            var permissionList = new List<PermissionAttribute>();

            foreach (var item in apis)
            {
                var attributes = item
                     .GetMethods()
                     .SelectMany(w => (PermissionAttribute[])w.GetCustomAttributes(typeof(PermissionAttribute), true));

                permissionList.AddRange(attributes);
            }

            return permissionList.Distinct()
                .Select(s => new PermissionItem { Feature = s.Feature, Action = s.Action })
                .ToList();
        }

        public static bool IsAllow(RenderContext context, Kooboo.Api.ApiMethod method)
        {
            // Append site Version for client synchronization. 
            if (context.WebSite != null)
            {
                var db = context.WebSite.SiteDb();
                var last = db.Log.Store.LastKey;

                context.Response.AppendCookie("_site_version_", last.ToString());
                context.Response.AppendCookie("_site_id_", context.WebSite.Id.ToString());
            }

            if (method.Permissions != null && method.Permissions.Count > 0)
            {
                if (context.User == null) return false;
                if (context.User.IsAdmin) return true;
                if (context.WebSite == null) return false;
                var sitedb = context.WebSite.SiteDb();
                if (context.WebSite.OrganizationId == context.User.Id) return true;

                var user = GetSiteUser(context);
                if (user == null) return false;

                var role = GetRole(sitedb, user.SiteRole);

                if (role == null) return false;
                return role.HasAccess(method.Permissions);
            }

            return true;
        }

        public static RolePermission GetRole(SiteDb siteDb, string role)
        {
            var rolePermission = siteDb.GetSiteRepository<RolePermissionRepository>().Get(role);

            if (rolePermission == null)
            {
                rolePermission = EmbeddedRoles.FirstOrDefault(f => f.Name == role);
            }

            Migrator.run(rolePermission, GetList());
            return rolePermission;
        }

        public static List<RolePermission> EmbeddedRoles { get; }

        public static RolePermission Master { get; } = new RolePermission
        {
            Name = "master",
            Permissions = GetList().Select(w => { w.Access = true; return w; }).ToList()
        };

        static readonly string[] _developerIncludes = new[] {
            Feature.CODE,
            Feature.CONTENT,
            Feature.CONTENT_TYPE,
            Feature.SPA_LANG,
            Feature.DATABASE,
            Feature.FORM,
            Feature.LAYOUT,
            Feature.MENU,
            Feature.PAGES,
            Feature.FRONT_EVENTS,
            Feature.CONFIG,
            Feature.FILE,
            Feature.HTML_BLOCK,
            Feature.JOB,
            Feature.KEY_VALUE,
            Feature.LABEL,
            Feature.MEDIA_LIBRARY,
            Feature.OPENAPI,
            Feature.STYLE,
            Feature.SCRIPT,
            Feature.SEARCH,
            Feature.LINK,
            Feature.MODULE,
            Feature.TEXT,
            Feature.VIEW,
            Feature.AUTHENTICATION,
            Feature.SITE,
            Feature.TABLE_RELATION,
        }.Select(s => s.ToLower()).ToArray();

        public static RolePermission Developer { get; } = new RolePermission
        {
            Name = "developer",
            Permissions = GetList().Select(w =>
            {
                if (_developerIncludes.Contains(w.Feature?.ToLower()))
                {
                    w.Access = true;
                }

                return w;
            }).ToList()
        };

        static readonly string[] _contentManagerIncludes = new[] {
            Feature.CONTENT,
            Feature.LABEL,
            Feature.HTML_BLOCK,
            Feature.FILE,
            Feature.TEXT,
            Feature.KEY_VALUE,
            Feature.MEDIA_LIBRARY,
            Feature.DATABASE,
            Feature.COMMERCE_PRODUCT,
            Feature.COMMERCE_PRODUCT_TYPE,
            Feature.COMMERCE_CATEGORY,
            Feature.USER_OPTIONS
        }.Select(s => s.ToLower()).ToArray();

        public static RolePermission ContentManager { get; } = new RolePermission
        {
            Name = "contentmanager",
            Permissions = GetList().Select(w =>
            {
                if (_contentManagerIncludes.Contains(w.Feature?.ToLower()))
                {
                    w.Access = true;
                }

                return w;
            }).ToList()
        };

        public static SiteUser GetSiteUser(RenderContext context)
        {
            if (context.User?.Id == default) return null;
            if (context.User.Id == BuiltInUser.CollaborationUser.Id)
            {
                var token = UserProviderHelper.GetJtwTokentFromContext(context);
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                var jwtDecoder = new JwtDecoder(serializer, urlEncoder);
                var payload = jwtDecoder.DecodeToObject(token);
                var siteId = payload["siteId"].ToString();
                if (context.WebSite != default)
                {
                    if (siteId != context.WebSite.Id.ToString()) return null;
                }
                return new SiteUser
                {
                    UserId = context.User.Id,
                    SiteRole = payload["role"].ToString()
                };
            }
            var user = context.WebSite.SiteDb().SiteUser.Get(context.User.Id);
            return user;
        }
    }
}