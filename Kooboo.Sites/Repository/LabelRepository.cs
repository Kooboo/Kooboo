//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB;

namespace Kooboo.Sites.Repository
{
    public class LabelRepository : SiteRepositoryBase<Label>
    {

        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.SetPrimaryKeyField<Label>(o => o.Id);
                return para; 
            }
        }

        public void UpdateLabel(Guid LabelGuid, string culture, string Value)
        {
           
            var label = Get(LabelGuid);

            if (label != null)
            {
                label.SetValue(culture, Value);
                if (this.WebSite.DefaultCulture == culture)
                {
                    label.SetValue("", Value); 
                }
                AddOrUpdate(label);
            }
        }
  
        public Label GetOrAdd(string LabelKey, string DefaultValue, string DefaultCulture)
        {
            var oldlable = GetByNameOrId(LabelKey);

            if (oldlable != null)
            {
                return oldlable;
            }
            else
            {
                Label newlabel = new Label();
                newlabel.Name = LabelKey;
                newlabel.SetValue(DefaultCulture, DefaultValue);
                AddOrUpdate(newlabel);
                return newlabel;
            }
        }
         
    }
}
