using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class CategoryService : ServiceBase<Category>
    {
        public TreeData Tree()
        {
            var all = this.Repo.List();
            var root = all.Where(o => o.ParentId == default(Guid)).ToList();
            TreeData top = new TreeData();
            SetSub(top, root, ref all);
            return top;
        }

        private void SetSub(TreeData Parent, List<Category> subs, ref List<Category> all)
        {
            if (subs == null || !subs.Any())
            {
                return;
            }

            foreach (var item in subs)
            {
                TreeData data = new TreeData();
                data.Key = item.Id.ToString();
                var value = item.GetValue(this.Context.Culture);
                if (value != null)
                {
                    data.Value = value.ToString();
                }

                Parent.Children.Add(data);

                var children = all.Where(o => o.ParentId == item.Id).ToList();

                SetSub(data, children, ref all);
            } 
        }

        private List<Category> Sub(Guid ParentId)
        {
            var list = this.Repo.List();
            return list.Where(o => o.ParentId == ParentId).ToList();
        } 

        /// <summary>
        ///  Key, guid or \root\sub\subsub like path. 
        /// </summary>
        /// <param name="ParentKeyOrIdOrPath"></param>
        /// <returns></returns>
        public List<Category> Sub(string ParentKeyOrIdOrPath)
        {
            if (ParentKeyOrIdOrPath == null)
            {
               return new List<Category>();
            } 
            var category = Get(ParentKeyOrIdOrPath);

            if (category != null)
            {
                return Sub(category.Id);
            } 
            return new List<Category>();
        }

        public override Category Get(string NameKeyIdOrPath)
        {
            if (NameKeyIdOrPath.Contains("/") || NameKeyIdOrPath.Contains("\\"))
            {
                return GetByPath(NameKeyIdOrPath);
            }
            else
            {
                var id = Lib.Helper.IDHelper.ParseKey(NameKeyIdOrPath);
                var item = this.Get(id);

                if (item != null)
                {
                    return item;
                }
                // else match by name. 

                var all = this.Repo.List();

                var find = all.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Name, NameKeyIdOrPath));

                return find;
            }
        }

        private Category GetByPath(string path)
        {
            if (path == null)
            {
                return null;
            }
            string[] parts = path.Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return GetByPath(parts);
        }

        private Category GetByPath(string[] path)
        {
            Category current = null;

            foreach (var item in path)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                List<Category> children = null;

                if (current == null)
                {
                    children = Top();
                }
                else
                {
                    children = Sub(current.Id);
                }

                var loweritem = item.ToLower();

                var find = children.Find(o => o.UserKey != null && o.UserKey.ToLower() == loweritem);

                if (find == null)
                {
                    find = children.Find(o => o.UserKey != null && o.UserKey.ToLower() == loweritem);
                }

                if (find == null)
                {
                    return null;
                }
                else
                {
                    current = find;
                }
            }

            return current;
        }

        public List<Category> Top()
        {
            // category is always cached...
            return this.Repo.List().Where(o => o.ParentId == default(Guid)).ToList();
        }
    }
}
