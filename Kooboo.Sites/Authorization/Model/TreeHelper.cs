using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Authorization.Model
{
    public static class TreeHelper
    {
        // fullrighgts like string[] of :
        // root/sub/sub
        // root/sub/subtwo
        public static PermissionTree GenerateTree(List<string> fullRights)
        {
            PermissionTree tree = new PermissionTree();
            foreach (var item in fullRights)
            {
                AppendToPermissionTree(tree, item);
            }
            return tree;
        }

        // root/sub/subtwo
        public static void AppendToPermissionTree(PermissionTree tree, string fullRightString)
        {
            var spe = "/\\".ToCharArray();
            var rights = fullRightString.Split(spe, StringSplitOptions.RemoveEmptyEntries);
            AppendToPermissionTree(tree, rights.ToList());
        }

        // single rights without \\
        public static void AppendToPermissionTree(PermissionTree tree, List<string> singleRights)
        {
            foreach (var item in singleRights)
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
    }
}