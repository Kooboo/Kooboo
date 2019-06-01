using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Authorization.Model;

namespace Kooboo.Sites.Authorization
{
    public static class PermissionTreeHelper
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
            List<string> rights = new List<string>();
            foreach (var item in HierarchyRights)
            {
                rights.Add(item);
            }
            return HasPermission(tree, rights);
        }

        public static bool HasPermission(PermissionTree tree, List<string> HierarchyRights)
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
         


        // root/sub/subtwo
        public static PermissionViewModel ToViewModel(RolePermission permission)
        {
            var spe = "/\\".ToCharArray();

            PermissionViewModel result = Kooboo.Sites.Authorization.ApiPermission.MasterTemplate();
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

                var find = model.SubItems.Find(o => o.Name == item);


                if (find == null)
                {
                    find = new PermissionViewModel();
                    model.SubItems.Add(find);

                    find.Name = item;
                }

                model = find;
            }

            model.Selected = true; // at the
        }


        public static List<string> ExtractFromModel(PermissionViewModel model)
        {
            return GetSubStrings(model.SubItems);
        }

        public static List<string> GetSubStrings(List<PermissionViewModel> SubModels)
        {
            List<string> result = new List<string>();

            foreach (var item in SubModels)
            {
                string root = item.Name;

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

            return result;
        }

    }
}
