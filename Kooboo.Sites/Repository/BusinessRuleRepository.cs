//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Repository
{
  public  class BusinessRuleRepository : SiteRepositoryBase<BusinessRule>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<BusinessRule>(it => it.Id);
                paras.AddColumn<BusinessRule>(it => it.EventType); 
                paras.SetPrimaryKeyField<BusinessRule>(o => o.Id);
                return paras;
            }
        } 

        public List<BusinessRule> ListByEventType(Kooboo.Sites.FrontEvent.enumEventType eventtype)
        {
            return this.List().FindAll(o=>o.EventType == eventtype).ToList(); 
        }
    }
     

 

}
