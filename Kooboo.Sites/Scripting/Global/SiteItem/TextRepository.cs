//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KScript.Sites;
using Kooboo.Data.Attributes;
using System.ComponentModel;

namespace KScript.Sites
{
    public class TextRepository : RepositoryBase
    {
        public TextRepository(IRepository repo, RenderContext context) : base(repo, context)
        {
        }

        [Description(@"Update text value of the body property
       var style = k.site.styles.getByUrl(""/a.css""); 
      k.site.styles.updateBody(style.Id, "".newcls{}"");")]
        public void UpdateBody(object NameOrId, string newbody)
        {
            var item = this.repo.GetByNameOrId(NameOrId.ToString());
            var domitem = item as ITextObject;
            domitem.Body = newbody;
            this.repo.AddOrUpdate(item);
        }

        [KIgnore]
        public object this[string key]
        {
            get
            {
                return Get(key);

            }
            set
            {
                this.AddOrUpdate(key, value);
            }
        }

        [KIgnore]
        protected virtual void AddOrUpdate(string key, object value)
        {
            throw new NotImplementedException();
            //if (value is ISiteObject)
            //{
            //    var sitevalue = value as ISiteObject;
            //    sitevalue.Name = key;
            //    this.Update(value);
            //}
            //else
            //{
            //    this.Add(value);
            //}
        }
    }
}
