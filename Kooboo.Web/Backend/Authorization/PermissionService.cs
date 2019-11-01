using Kooboo.Data.Context;
using Kooboo.Sites.Authorization.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Authorization
{
    public static class PermissionService
    {
        public static bool HasPermission(PermissionTree tree, params string[] hierarchyRights)
        {
            return HasPermission(hierarchyRights, tree);
        }

        public static bool HasPermission(string[] hierarchyRights, PermissionTree tree)
        {
            foreach (var item in hierarchyRights)
            {
                if (tree.RootAccess)
                {
                    return true;
                }

                if (tree.Children.ContainsKey(item))
                {
                    tree = tree.Children[item];
                }
                else
                {
                    return false;
                }
            }

            return tree.RootAccess;
        }

        public static bool HasPermission(string permissionstring, PermissionTree tree)
        {
            var spe = "/\\".ToCharArray();

            var paras = permissionstring.Split(spe, StringSplitOptions.RemoveEmptyEntries);

            return HasPermission(paras, tree);
        }

        // root/sub/subtwo
        public static PermissionViewModel ToViewModel(RolePermission permission, RenderContext context)
        {
            var spe = "/\\".ToCharArray();

            PermissionViewModel result = Kooboo.Sites.Authorization.DefaultData.Available;
            result.Name = permission.Name;
            result.Id = permission.Id;

            //result.DisplayName = Kooboo.Data.Language.LanguageProvider.GetValue(result.Name, context);

            foreach (var item in permission.Permission)
            {
                var rights = item.Split(spe, StringSplitOptions.RemoveEmptyEntries);
                AppendToModel(result, rights.ToList());
            }

            SetDisplayName(result.SubItems, context);
            return result;
        }

        private static void SetDisplayName(List<PermissionViewModel> subitems, RenderContext context)
        {
            foreach (var item in subitems)
            {
                item.DisplayName = Data.Language.LanguageProvider.GetValue(item.Name, context);

                if (item.SubItems.Any())
                {
                    SetDisplayName(item.SubItems, context);
                }
            }
        }

        // single rights without \\
        public static void AppendToModel(PermissionViewModel model, List<string> singleRights)
        {
            foreach (var item in singleRights)
            {
                if (model.Selected)
                {
                    return;
                }

                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                var find = model.SubItems.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Name, item));

                if (find == null)
                {
                    return;
                }

                model = find;
            }

            model.Selected = true; // at the
        }

        public static List<string> ExtractPermissionFromModel(PermissionViewModel model)
        {
            return GetSubStrings(model.SubItems);
        }

        private static List<string> GetSubStrings(List<PermissionViewModel> subModels)
        {
            List<string> result = new List<string>();

            foreach (var item in subModels)
            {
                string root = item.Name;

                if (item.Selected)
                {
                    result.Add(root);
                }
                else
                {
                    if (item.SubItems != null && item.SubItems.Any())
                    {
                        var substrings = GetSubStrings(item.SubItems);
                        foreach (var str in substrings)
                        {
                            string rootsub = root + "/" + str;
                            result.Add(rootsub);
                        }
                    }
                }
            }

            return result;
        }
    }
}