//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Repository
{
    public class HtmlBlockRepository : SiteRepositoryBase<HtmlBlock>
    {

        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<HtmlBlock>(it => it.Id);
                paras.AddColumn<HtmlBlock>(it => it.Name);
                paras.AddColumn<HtmlBlock>(it => it.LastModified);
                paras.SetPrimaryKeyField<HtmlBlock>(o => o.Id);
                return paras;
            }
        }

        public void AddOrUpdate(string name, Dictionary<string, string> values, Guid HtmlBlockId = default(Guid))
        {
            HtmlBlock newblock = new HtmlBlock();
            newblock.Name = name;
            foreach (var item in values)
            {
                newblock.Values[item.Key] = item.Value;
            }

            if (HtmlBlockId == default(Guid))
            {
                AddOrUpdate(newblock);
            }
            else
            {
                AddOrUpdate(newblock);
            }
        }

        public HtmlBlock Get(string nameOrId)
        {
            return GetByNameOrId(nameOrId); 
        }

        public HtmlBlock GetOrAdd(string blockName, string DefaultValue, string DefaultCulture)
        {
            var old = GetByNameOrId(blockName);

            if (old != null)
            {
                return old;
            }
            else
            {
                HtmlBlock block = new HtmlBlock();
                block.Name = blockName;
                block.SetValue(DefaultCulture, DefaultValue);
                AddOrUpdate(block);
                return block;
            }
        }

    }
}
