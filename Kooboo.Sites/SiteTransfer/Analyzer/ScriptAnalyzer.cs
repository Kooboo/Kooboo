//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System.Text.RegularExpressions;

namespace Kooboo.Sites.SiteTransfer
{
    public class ScriptAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext Context)
        {
            HTMLCollection scripts = Context.Dom.getElementsByTagName("script");

            foreach (var item in scripts.item)
            {
                if (item.hasAttribute("src"))
                {
                    string srcurl = Service.DomUrlService.GetLinkOrSrc(item);

                    if (string.IsNullOrEmpty(srcurl))
                    {
                        /// script tag with a src source. does not consider as a script. 
                        continue;
                    }

                    string fullurl = UrlHelper.Combine(Context.AbsoluteUrl, srcurl);

                    bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(Context.OriginalImportUrl, fullurl);

                    if (issamehost)
                    {

                        string relativeurl = UrlHelper.RelativePath(fullurl, issamehost);
                        relativeurl = TransferHelper.TrimQuestionMark(relativeurl);

                        if (srcurl != relativeurl)
                        {
                            string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                            string newstring = oldstring.Replace(srcurl, relativeurl);
                            Context.Changes.Add(new AnalyzerUpdate()
                            {
                                StartIndex = item.location.openTokenStartIndex,
                                EndIndex = item.location.openTokenEndIndex,
                                NewValue = newstring
                            });
                        }


                        Context.DownloadManager.AddTask(new Download.DownloadTask()
                        {
                            AbsoluteUrl = fullurl,
                            RelativeUrl = relativeurl,
                            ConstType = ConstObjectType.Script,
                            OwnerObjectId = Context.ObjectId
                        });
                    }
                }
                else
                {
                    ///<script>if (document.location.protocol != "https:") {document.location = document.URL.replace(/^http:/i, "https:");}</script>

                    string text = item.InnerHtml;
                    if (!string.IsNullOrWhiteSpace(text) && text.Length < 200)
                    {
                        var lower = text.ToLower();
                        var hasProtocolOperator = lower.Contains("document.location.protocol") && lower.Contains("document.url.replace") && lower.Contains("https");
                        var hasLocationOperator = Regex.IsMatch(lower, "window.top.location\\s*=");
                        if (hasProtocolOperator || hasLocationOperator)
                        {

                            Context.Changes.Add(new AnalyzerUpdate()
                            {
                                StartIndex = item.location.openTokenStartIndex,
                                EndIndex = item.location.endTokenEndIndex,
                                NewValue = "<script>/* https redirect removed */</script>"
                            });
                        }
                    }


                    //string text = item.InnerHtml;
                    //if (!string.IsNullOrEmpty(text))
                    //{
                    //    // this is an embedded script. 
                    //    var script = new Script
                    //    {
                    //        IsEmbedded = true,
                    //        Body = text,
                    //        OwnerObjectId = Context.ObjectId,
                    //        OwnerConstType = Context.ObjectType,
                    //        ItemIndex = embeddedItemIndex,
                    //        Name = UrlHelper.FileName(Context.AbsoluteUrl)
                    //    };

                    //    embeddedItemIndex += 1;

                    //    Context.SiteDb.Scripts.AddOrUpdate(script);

                    //}
                }


            }
        }
    }
}
