//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.SiteTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Repository
{
    public class TransferPageRepository : SiteRepositoryBase<TransferPage>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<TransferPage>(o => o.taskid);
                paras.AddColumn<TransferPage>(o => o.done);
                paras.AddColumn<TransferPage>(o => o.HtmlSourceHash);
                paras.AddColumn<TransferPage>(o => o.CreationDate);
                paras.SetPrimaryKeyField<TransferPage>(o => o.Id);
                return paras;
            }
        }

    }
}
