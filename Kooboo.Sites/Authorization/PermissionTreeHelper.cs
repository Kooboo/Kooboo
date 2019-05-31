using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
    }
}
