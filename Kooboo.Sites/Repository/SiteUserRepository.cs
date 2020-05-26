using Kooboo.Sites.Models;
using System;

namespace Kooboo.Sites.Repository
{
 public   class SiteUserRepository : SiteRepositoryBase<SiteUser>
    { 
        public override bool AddOrUpdate(SiteUser value)
        {
            return this.AddOrUpdate(value, default(Guid));
        }

        public override bool AddOrUpdate(SiteUser value, Guid UserId)
        {
            var ok = base.AddOrUpdate(value, UserId);
            this.Store.Close();
            return ok; 
        }

        public override void Delete(Guid id)
        {
            this.Delete(id,default(Guid));
        }

        public override void Delete(Guid id, Guid UserId)
        {
            base.Delete(id, UserId);
            this.Store.Close(); 
        }
    }
}
