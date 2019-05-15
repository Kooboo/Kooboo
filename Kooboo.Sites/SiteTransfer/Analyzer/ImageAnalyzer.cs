//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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
            var imgurls = GetImageUrls(Context.Dom.images.item); 
           
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

                                Context.SiteDb.Routes.AddOrUpdate(url, ConstObjectType.Image, koobooimage.Id, Context.DownloadManager.UserId);

                                Context.SiteDb.Images.AddOrUpdate(koobooimage, Context.DownloadManager.UserId);


                                string oldstring = item.Key.OuterHtml;

                                string newstring = oldstring.Replace(itemsrc, url);

                                Context.Changes.Add(new AnalyzerUpdate()
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
                        string absoluteUrl = Kooboo.Lib.Helper.UrlHelper.Combine(Context.AbsoluteUrl, itemsrc);

                        bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(absoluteUrl, Context.OriginalImportUrl);

                        string relativeurl = EusureUrl(UrlHelper.RelativePath(absoluteUrl, issamehost));

                        if (itemsrc != relativeurl)
                        {
                            string oldstring = item.Key.OuterHtml;

                            string newstring = oldstring.Replace(itemsrc, relativeurl);

                            Context.Changes.Add(new AnalyzerUpdate()
                            {
                                StartIndex = item.Key.location.openTokenStartIndex,
                                EndIndex = item.Key.location.openTokenEndIndex,
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

        public Dictionary<Kooboo.Dom.Element, string> GetImageUrls(List<Kooboo.Dom.Element> imagetags)
        {
            Dictionary<Kooboo.Dom.Element, string> result = new Dictionary<Dom.Element, string>();

            foreach (var item in imagetags)
            {
                var url = item.getAttribute("src");

                if (string.IsNullOrWhiteSpace(url))
                {
                    url = GetNonSrcUrl(item);
                }

                if (!string.IsNullOrWhiteSpace(url))
                {
                    result.Add(item, url);
                }
            }

            return EnsurePossibleNonSrc(result);
        }


        public Dictionary<Kooboo.Dom.Element, string> EnsurePossibleNonSrc(Dictionary<Kooboo.Dom.Element, string> orgResult)
        {
            // when all values are the same, possible non-src is the right value. 

            if (orgResult.Count >= 2)
            {
                if (IsSameValue(orgResult.Values))
                {
                    Dictionary<Kooboo.Dom.Element, string> newresult = new Dictionary<Dom.Element, string>();

                    foreach (var item in orgResult)
                    {
                        var value = GetNonSrcUrl(item.Key); 
                        if (!string.IsNullOrWhiteSpace(value) && !Lib.Helper.StringHelper.IsSameValue(value, item.Value))
                        {
                            newresult[item.Key] = value; 
                        }
                        else
                        {
                            newresult[item.Key] = item.Value; 
                        }
                    } 
                    return newresult; 
                }
            }

            return orgResult; 
        }


        public bool IsSameValue(IEnumerable<string> values)
        {
            bool issame = true;
            string value = null;
            foreach (var item in values)
            {
                if (value == null)
                {
                    value = item;
                }
                else
                {   
                    if (!Lib.Helper.StringHelper.IsSameValue(value, item))
                    {
                        return false; 
                    } 
                }
            }

            return issame;
        }
         
        public string GetNonSrcUrl(Kooboo.Dom.Element imagetag)
        {
            if (imagetag == null)
            {
                return null; 
            }

            foreach (var item in imagetag.attributes)
            {
                if (item != null && item.name != null)
                {
                    string name = item.name.Trim().ToLower();
                    if (name != "src" && name.Contains("src"))
                    {
                        return item.value;
                    }
                }
            }
            return null;
        } 
    }


}
