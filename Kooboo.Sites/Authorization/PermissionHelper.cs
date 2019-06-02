using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Authorization.Model;

namespace Kooboo.Sites.Authorization
{
    public static class PermissionHelper
    {
        // fullrighgts like string[] of :
        // root/sub/sub
        // root/sub/subtwo
        public static PermissionTree GenerateTree(List<string> FullRights)
        {
            PermissionTree tree = new PermissionTree();
            foreach (var item in FullRights)
            {
                AppendToPermissionTree(tree, item);
            }
            return tree;
        }

        // root/sub/subtwo
        public static void AppendToPermissionTree(PermissionTree tree, string FullRightString)
        {
            var spe = "/\\".ToCharArray();
            var rights = FullRightString.Split(spe, StringSplitOptions.RemoveEmptyEntries);
            AppendToPermissionTree(tree, rights.ToList());
        }

        // single rights without \\ 
        public static void AppendToPermissionTree(PermissionTree tree, List<string> SingleRights)
        {
            foreach (var item in SingleRights)
            {
                if (tree.RootAccess)
                {
                    return;
                }

                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                if (tree.Children.ContainsKey(item))
                {
                    tree = tree.Children[item];
                }
                else
                {
                    // append new one...
                    PermissionTree subtree = new PermissionTree();
                    tree.Children[item] = subtree;
                    tree.Name = item;
                    tree = subtree;
                }
            }

            tree.RootAccess = true; // at the
        }

        public static bool HasPermission(PermissionTree tree, params string[] HierarchyRights)
        { 
            return HasPermission(HierarchyRights, tree);
        }

        public static bool HasPermission(string[] HierarchyRights, PermissionTree tree)
        {
            foreach (var item in HierarchyRights)
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
        public static PermissionViewModel ToViewModel(RolePermission permission)
        {
            var spe = "/\\".ToCharArray();

            PermissionViewModel result = Kooboo.Sites.Authorization.DefaultData.Available;
            result.Name = permission.Name;
            result.Id = permission.Id;

            foreach (var item in permission.Permission)
            {
                var rights = item.Split(spe, StringSplitOptions.RemoveEmptyEntries);
                AppendToModel(result, rights.ToList());
            }
            return result;
        }
         
        // single rights without \\ 
        public static void AppendToModel(PermissionViewModel model, List<string> SingleRights)
        {
            foreach (var item in SingleRights)
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

        private static List<string> GetSubStrings(List<PermissionViewModel> SubModels)
        {
            List<string> result = new List<string>();

            foreach (var item in SubModels)
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
