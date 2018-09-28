using System;
using System.Collections.Generic;
using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System.Linq;    

namespace Kooboo.Sites.Ecommerce.Repository
{
   public class CategoryRepository : SiteRepositoryBase<Category>
    {

        internal override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddColumn<Category>(o => o.ParentId); 
                return para;
            }
        }

        public override void Delete(Guid id)
        {           
            var allcate = this.SiteDb.Category.All();
            _delete(id, ref allcate); 
        }

        private void _delete(Guid id, ref List<Category> all)
        {
            var subs = all.Where(o => o.ParentId == id);

            foreach (var item in subs)
            {
                _delete(item.Id, ref all); 
            }   
            base.Delete(id); 
        }
    }     


}
