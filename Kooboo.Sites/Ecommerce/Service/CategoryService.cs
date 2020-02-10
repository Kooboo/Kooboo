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

        public List<Category> Sub(Guid ParentId)
        {
            var list = this.Repo.All();
            return list.Where(o => o.ParentId == ParentId).ToList();  
        } 

        public List<Category> Top()
        {
            // category is always cached...
            var list = this.Repo.All();
            return list.Where(o => o.ParentId == default(Guid)).ToList();  
        }  
    }
}
