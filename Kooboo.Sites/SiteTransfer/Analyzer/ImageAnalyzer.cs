using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Routing;
using Kooboo.Extensions;
using Kooboo.Sites.Repository; 

namespace Kooboo.Sites.SiteTransfer
{
    public class ImageAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext Context)
        {
           // Dictionary<string, bool> ImageUrlsToDownload = new Dictionary<string, bool>();  

            foreach (var item in Context.Dom.images.item)
            {
                string itemsrc = DomUrlService.GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(itemsrc))
                {
                    if (Kooboo.Lib.Utilities.DataUriService.isDataUri(itemsrc))
                    {
                        var datauri = Kooboo.Lib.Utilities.DataUriService.PraseDataUri(itemsrc);

                        if (datauri != null)
                        {
                            if (datauri.isBase64)
                            {
                                Guid newid = itemsrc.ToHashGuid();

                                Image koobooimage = new Image
                                {
                                    Extension = UrlHelper.GetImageExtensionFromMine(datauri.MineType),
                                    ContentBytes = Convert.FromBase64String(datauri.DataString),
                                    Id = newid,
                                    Name = newid.ToString()
                                };

                                string url = "/image/base64/page/" + koobooimage.Id.ToString();

                                Context.SiteDb.Routes.AddOrUpdate(url, ConstObjectType.Image, koobooimage.Id, Context.DownloadManager.UserId);

                                Context.SiteDb.Images.AddOrUpdate(koobooimage, Context.DownloadManager.UserId);
                                 

                                string oldstring = item.OuterHtml;

                                string newstring = oldstring.Replace(itemsrc, url);

                                Context.Changes.Add(new AnalyzerUpdate()
                                {
                                    StartIndex = item.location.openTokenStartIndex,
                                    EndIndex = item.location.openTokenEndIndex,
                                    NewValue = newstring
                                });


                            }
                            else
                            {
                                // TODO: other encoding not implemented yet. 
                            }

                        }

                        continue;

                    }

                    else
                    {
                        string absoluteUrl = Kooboo.Lib.Helper.UrlHelper.Combine(Context.AbsoluteUrl, itemsrc);

                        bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(absoluteUrl, Context.OriginalImportUrl);
                         
                        string relativeurl = EnsureUrlWithoutQuestionMark(UrlHelper.RelativePath(absoluteUrl, issamehost));

                        if (itemsrc != relativeurl)
                        {
                            string oldstring = item.OuterHtml;

                            string newstring = oldstring.Replace(itemsrc, relativeurl);

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
                            ConstType = ConstObjectType.Image,
                            OwnerObjectId = Context.ObjectId
                        });

                    }
                }
            }  
        } 

        public string EnsureUrlWithoutQuestionMark(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return null; 
            }

            if (relativeUrl.IndexOf("?")>-1)
            {
                return relativeUrl.Replace("?", "/"); 
            } 
            else
            {
                return relativeUrl; 
            }  
        }
    }
     

}
