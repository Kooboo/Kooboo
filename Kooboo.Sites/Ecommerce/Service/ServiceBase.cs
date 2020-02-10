using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class ServiceBase<T> : IEcommerceService<T> where T : class, ISiteObject
    {
        public RenderContext Context { get; set; }
        public CommerceContext CommerceContext { get; set; }

        public long Priority => 1;

        public SiteRepositoryBase<T> Repo
        {
            get
            {
                if (Context.WebSite != null)
                {
                    var sitedb = Context.WebSite.SiteDb();

                    var repo = sitedb.GetSiteRepositoryByModelType(typeof(T)) as SiteRepositoryBase<T>;

                    return repo;
                }
                return null;
            }
        }

        public virtual bool AddOrUpdate(T value)
        {
            return this.Repo.AddOrUpdate(value, default(Guid));
        }

        public virtual bool AddOrUpdate(T value, Guid UserId)
        {
            return this.Repo.AddOrUpdate(value, UserId);
        }

        public virtual void Delete(Guid id, Guid UserId)
        {
            this.Repo.Delete(id, UserId);
        }

        public virtual void Delete(Guid id)
        {
            this.Repo.Delete(id, default(Guid));
        }

        public virtual T Get(Guid id)
        {
            return this.Repo.Get(id);
        }
         
        public virtual async Task<T> GetAsync(Guid id)
        {
            return await this.Repo.GetAsync(id);
        }

        public virtual T Get(string nameorid)
        {
            return this.Repo.GetByNameOrId(nameorid);
        }
         
    }
}
