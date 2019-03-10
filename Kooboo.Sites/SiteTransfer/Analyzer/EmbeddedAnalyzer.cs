//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
 using Kooboo.Lib.Helper;
 

namespace Kooboo.Sites.SiteTransfer
{
    public class EmbeddedAnalyzer : ITransferAnalyzer
    { 
        public void Execute(AnalyzerContext Context)
        {
            HTMLCollection embedElement = Context.Dom.getElementsByTagName("embed");

            foreach (var item in embedElement.item)
            {
                string fileurl =Kooboo.Sites.Service.DomUrlService.GetLinkOrSrc(item);

                if (string.IsNullOrEmpty(fileurl))
                {
                    continue;
                }

                string absoluteUrl = UrlHelper.Combine(Context.AbsoluteUrl, fileurl);

                if (!isDownloadAble(absoluteUrl))
                {
                    /// if not going to download, just change the url. 
                    if (fileurl != absoluteUrl)
                    {
                        string oldstring = Service.DomService.GetOpenTag(item);
                        string newstring = oldstring.Replace(fileurl, absoluteUrl);
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
                    bool issamehost = UrlHelper.isSameHost(Context.OriginalImportUrl, absoluteUrl);

                    if (issamehost)
                    {

                        string relativeurl = UrlHelper.RelativePath(absoluteUrl, issamehost);

                        if (fileurl != relativeurl)
                        {
                            string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                            string newstring = oldstring.Replace(fileurl, relativeurl);
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
                            ConstType = ConstObjectType.CmsFile,
                            OwnerObjectId = Context.ObjectId
                        });
                    } 
                }

            }

        }
         
        private bool isDownloadAble(string url)
        { 
            if (string.IsNullOrEmpty(url))
            {
                return false; 
            }
            url = url.ToLower();

            return (url.EndsWith(".flv") || url.EndsWith(".swf"));
        }

    }
}