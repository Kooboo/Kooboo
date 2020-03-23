//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Sites.Routing;
using Kooboo.Extensions;


namespace Kooboo.Sites.SiteTransfer
{
    public class CSSAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext Context)
        {
            ProcessInPage(Context);
            ProcessExternal(Context);
           // ProcessInline(Context);
        }

        /// <summary>
        /// process the embedded style.
        /// </summary>
        /// <param name="context"></param>
        private void ProcessInPage(AnalyzerContext context)
        {
            HTMLCollection embedStyle = context.Dom.getElementsByTagName("style");

            //int itemindexcounter = 0;
            foreach (var item in embedStyle.item)
            {
                string csstext = item.InnerHtml;

                if (string.IsNullOrEmpty(csstext))
                {
                    continue;
                } 
                //var style = new Style
                //{
                //    IsEmbedded = true,
                //    OwnerObjectId = context.ObjectId,
                //    OwnerConstType = context.ObjectType,
                //    ItemIndex = itemindexcounter,
                //    Name = UrlHelper.FileName(context.AbsoluteUrl)
                //};

                CssManager.ProcessResource(ref csstext, context.AbsoluteUrl, context.DownloadManager, context.ObjectId); 

                //style.Body = csstext; 
                //context.SiteDb.Styles.AddOrUpdate(style, context.DownloadManager.UserId); 
                //itemindexcounter += 1;
                
                if (item.InnerHtml != csstext)
                {
                    var change = new AnalyzerUpdate()
                    {
                        StartIndex = item.location.openTokenEndIndex + 1,
                        EndIndex = item.location.endTokenStartIndex - 1,
                        NewValue = csstext
                    }; 

                    if (change.EndIndex > change.StartIndex)
                    {
                        context.Changes.Add(change);
                    } 
                }
            }
        }

        private void ProcessExternal(AnalyzerContext Context)
        {
            HTMLCollection styletags = Context.Dom.getElementsByTagName("link");

            foreach (var item in styletags.item)
            {
                if (item.hasAttribute("rel") && item.getAttribute("rel").ToLower().Contains("stylesheet"))
                {
                    string itemurl = Service.DomUrlService.GetLinkOrSrc(item);

                    if (!string.IsNullOrEmpty(itemurl))
                    {
                       /// itemurl = TransferHelper.TrimQuestionMark(itemurl);

                        string absoluteUrl = UrlHelper.Combine(Context.AbsoluteUrl, itemurl);

                        bool issamehost = UrlHelper.isSameHost(Context.OriginalImportUrl, absoluteUrl);

                        string relativeurl = UrlHelper.RelativePath(absoluteUrl, issamehost);

                        if (itemurl != relativeurl)
                        {
                            string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                            string newstring = oldstring.Replace(itemurl, relativeurl);
                            Context.Changes.Add(new AnalyzerUpdate()
                            {
                                StartIndex = item.location.openTokenStartIndex,
                                EndIndex = item.location.openTokenEndIndex,
                                NewValue = newstring
                            });
                        }

                        Context.DownloadManager.AddTask(new Download.DownloadTask()
                        {
                            AbsoluteUrl = absoluteUrl,
                            RelativeUrl = relativeurl,
                            ConstType = ConstObjectType.Style,
                            OwnerObjectId = Context.ObjectId
                        });
                    }

                }
            }
        }
          
    }

}
