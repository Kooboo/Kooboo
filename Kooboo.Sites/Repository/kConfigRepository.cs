using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Repository
{
    public class kConfigRepository : SiteRepositoryBase<KConfig>
    {
                           
        internal override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.SetPrimaryKeyField<KConfig>(o => o.Id);
                return para;
            }
        }

        public KConfig GetOrAdd(string key, string TagName,  string TagHtml)
        {
            var old = GetByNameOrId(key);

            if (old != null)
            {
                return old;
            }
            else
            {
                KConfig config = new KConfig();
                config.Name = key;
                config.TagHtml = TagHtml;
                config.TagName = TagName;    
                AddOrUpdate(config);
                return config;
            }
        }        
    }


}
