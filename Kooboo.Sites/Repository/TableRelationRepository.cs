using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Repository
{ 
    public class TableRelationRepository : SiteRepositoryBase<TableRelation>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters(); 
                para.SetPrimaryKeyField<TableRelation>(o => o.Id);
                return base.StoreParameters;
            }
        }
    }

}
