using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Authorization.Model
{
  public static  class TreeHelper
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



    }
}


