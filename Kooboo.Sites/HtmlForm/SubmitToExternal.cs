//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.HtmlForm
{               
    public class SubmitToExternal : Data.Interface.IFormSubmitter
    {
        public string Name
        {
            get { return "SubmitToExternal"; }
        }

        public string CustomActionUrl(RenderContext context, Dictionary<string, string> settings)
        {
            return settings.First().Value;
        }

        public List<SimpleSetting> Settings(RenderContext context)
        {
            var setting = new List<SimpleSetting>();
            SimpleSetting item = new SimpleSetting();
            item.Name = "Url";
            item.ControlType = Data.ControlType.Selection;

            var sitedb = context.WebSite.SiteDb();

            var allpages = sitedb.Pages.All();

            foreach (var page in allpages)
            {
                var url = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, page);

                if (url != null)
                {
                    if (url.Contains("{") && url.Contains("}"))
                    {
                        continue;
                    }
                    else
                    {
                        item.SelectionValues[url] = url;
                    }
                }    
            }
            setting.Add(item);
            return setting;
        }

        public bool Submit(RenderContext context, Guid FormId, Dictionary<string, string> settings)
        {
            // this will post directly to the url... 
            return true;
        }
    }


}
