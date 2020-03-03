using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository; 

namespace Kooboo.Sites.Ecommerce.Repository
{
  
    public class PromotionRuleRepository : SiteRepositoryBase<PromotionRule>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<ProductVariants>(it => it.Id);
                paras.AddColumn<ProductVariants>(it => it.Online);
                paras.AddColumn<ProductVariants>(it => it.LastModified);
                paras.AddIndex<ProductVariants>(it => it.ProductId);
                paras.SetPrimaryKeyField<ProductVariants>(o => o.Id);
                return paras;
            }
        } 

    }






}
