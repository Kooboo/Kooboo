using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
  public class TextRepository   : RepositoryBase
    {
        public TextRepository(IRepository repo, RenderContext context) : base(repo, context)
        {             
        }
           
        public void UpdateBody(object NameOrId, string newbody)
        {
            var item = this.repo.GetByNameOrId(NameOrId.ToString());
            var domitem = item as ITextObject;
            domitem.Body = newbody;
            this.repo.AddOrUpdate(item); 
        }   
    }
}
