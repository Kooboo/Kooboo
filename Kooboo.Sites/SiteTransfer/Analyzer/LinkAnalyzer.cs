//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Data;
using Kooboo.Sites.Routing;

namespace Kooboo.Sites.SiteTransfer
{
    public class LinkAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext Context)
        {
            foreach (var item in Context.Dom.Links.item)
            {
                string itemsrc = Service.DomUrlService.GetLinkOrSrc(item);

                if (string.IsNullOrEmpty(itemsrc))
                {
                    continue;
                }

                string absoluteurl = UrlHelper.Combine(Context.AbsoluteUrl, itemsrc);

                bool issamehost = UrlHelper.isSameHost(absoluteurl, Context.OriginalImportUrl);

                var objectType = Service.ConstTypeService.GetConstTypeByUrl(absoluteurl);

                if (issamehost)
                {
                    string relativeurl = UrlHelper.RelativePath(absoluteurl, issamehost);

                    if (itemsrc != relativeurl)
                    {
                        string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                        string newstring = oldstring.Replace(itemsrc, relativeurl);
                        Context.Changes.Add(new AnalyzerUpdate()
                        {
                            StartIndex = item.location.openTokenStartIndex,
                            EndIndex = item.location.openTokenEndIndex,
                            NewValue = newstring
                        });
                    } 
                }
                else
                {
                    if (itemsrc != absoluteurl)
                    {
                        string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                        string newstring = oldstring.Replace(itemsrc, absoluteurl);
                        Context.Changes.Add(new AnalyzerUpdate()
                        {
                            StartIndex = item.location.openTokenStartIndex,
                            EndIndex = item.location.openTokenEndIndex,
                            NewValue = newstring
                        });
                    }
                }
            }
        }
    }
}
