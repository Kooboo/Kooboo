//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System;

namespace Kooboo.Sites.SiteTransfer
{
    public class ImageAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext context)
        {
            var imgurls = Kooboo.Sites.Service.DomUrlService.GetImageSrcs(context.Dom);

            foreach (var item in imgurls)
            {
                string itemsrc = item.Value;

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

                                context.SiteDb.Routes.AddOrUpdate(url, ConstObjectType.Image, koobooimage.Id, context.DownloadManager.UserId);

                                context.SiteDb.Images.AddOrUpdate(koobooimage, context.DownloadManager.UserId);

                                string oldstring = item.Key.OuterHtml;

                                string newstring = oldstring.Replace(itemsrc, url);

                                context.Changes.Add(new AnalyzerUpdate()
                                {
                                    StartIndex = item.Key.location.openTokenStartIndex,
                                    EndIndex = item.Key.location.openTokenEndIndex,
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
                        string absoluteUrl = Kooboo.Lib.Helper.UrlHelper.Combine(context.AbsoluteUrl, itemsrc);

                        bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(absoluteUrl, context.OriginalImportUrl);

                        string relativeurl = EusureUrl(UrlHelper.RelativePath(absoluteUrl, issamehost));

                        if (itemsrc != relativeurl)
                        {
                            string oldstring = item.Key.OuterHtml;

                            string newstring = oldstring.Replace(itemsrc, relativeurl);

                            context.Changes.Add(new AnalyzerUpdate()
                            {
                                StartIndex = item.Key.location.openTokenStartIndex,
                                EndIndex = item.Key.location.openTokenEndIndex,
                                NewValue = newstring
                            });
                        }

                        context.DownloadManager.AddTask(new Download.DownloadTask()
                        {
                            AbsoluteUrl = absoluteUrl,
                            RelativeUrl = relativeurl,
                            ConstType = ConstObjectType.Image,
                            OwnerObjectId = context.ObjectId
                        });
                    }
                }
            }
        }

        public string EusureUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return null;
            }

            return System.Net.WebUtility.UrlDecode(relativeUrl);
            //if (relativeUrl.IndexOf("?") > -1)
            //{
            //    return relativeUrl.Replace("?", "/");
            //}
            //else
            //{
            //    return relativeUrl;
            //}
        }
    }
}