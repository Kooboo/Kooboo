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
  
    public class CodeRepository : IEmbeddableRepositoryBase<Code>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<Code>(it => it.Id);
                paras.AddColumn<Code>(it => it.EventType);
                paras.AddColumn<Code>(it => it.CodeType);

                paras.AddColumn<Code>(it => it.OwnerObjectId);
                paras.AddColumn<Code>(it => it.OwnerConstType);
                paras.AddColumn<Code>(it => it.BodyHash);
                paras.AddColumn<Code>(it => it.Id);
                paras.AddColumn<Code>(it => it.IsEmbedded);
                paras.AddColumn<Code>(it => it.LastModified);
                paras.AddColumn("Name", 100);

                paras.SetPrimaryKeyField<Code>(o => o.Id);
                 

                return paras;
            }
        }

        public List<Code> GetByEvent(FrontEvent.enumEventType eventtype)
        {
            return this.Query.Where(o => o.CodeType == CodeType.Event && o.EventType == eventtype).SelectAll(); 
        }

        public List<Code> ListByCodeType(CodeType codetype)
        {
            return this.Query.Where(o => o.CodeType == codetype).SelectAll();
        }
    }
}
