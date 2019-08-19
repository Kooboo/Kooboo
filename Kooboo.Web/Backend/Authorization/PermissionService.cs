using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Authorization.Model;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Authorization
{
    public static class PermissionService
    { 
      
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

        private static void SetDisplayName(List<PermissionViewModel> Subitems, RenderContext context)
        {
            foreach (var item in Subitems)
            {
                item.DisplayName = Data.Language.LanguageProvider.GetValue(item.Name, context); 

                if (item.SubItems.Any())
                {
                    SetDisplayName(item.SubItems, context); 
                }
            }
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
