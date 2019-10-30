//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using System;

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

        public void UpdateLabel(Guid labelGuid, string culture, string value)
        {
            var label = Get(labelGuid);

            if (label != null)
            {
                label.SetValue(culture, value);
                if (this.WebSite.DefaultCulture == culture)
                {
                    label.SetValue("", value);
                }
                AddOrUpdate(label);
            }
        }

        public Label GetOrAdd(string labelKey, string defaultValue, string defaultCulture)
        {
            var oldlable = GetByNameOrId(labelKey);

            if (oldlable != null)
            {
                return oldlable;
            }
            else
            {
                Label newlabel = new Label {Name = labelKey};
                newlabel.SetValue(defaultCulture, defaultValue);
                AddOrUpdate(newlabel);
                return newlabel;
            }
        }
    }
}