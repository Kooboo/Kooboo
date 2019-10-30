//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;

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

        public void AddOrUpdate(string name, Dictionary<string, string> values, Guid htmlBlockId = default(Guid))
        {
            HtmlBlock newblock = new HtmlBlock {Name = name};
            foreach (var item in values)
            {
                newblock.Values[item.Key] = item.Value;
            }

            if (htmlBlockId == default(Guid))
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

        public HtmlBlock GetOrAdd(string blockName, string defaultValue, string defaultCulture)
        {
            var old = GetByNameOrId(blockName);

            if (old != null)
            {
                return old;
            }
            else
            {
                HtmlBlock block = new HtmlBlock {Name = blockName};
                block.SetValue(defaultCulture, defaultValue);
                AddOrUpdate(block);
                return block;
            }
        }
    }
}