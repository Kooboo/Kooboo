//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Image = Kooboo.Sites.Models.Image;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Service
{
    public static class ThumbnailService
    {
        /// <summary>
        /// The thumbnail must be in a fixed format of 
        /// /thumbnail/200/200/guidorpath. 
        /// TODO: Refactor to new location, do not know where to put this file yet. 
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static ThumbnailRequest ParseThumbnailRequest(string relativeUrl)
        {
            ThumbnailRequest request = new ThumbnailRequest();

            if (relativeUrl == null)
            {
                return request;
            }

            var pathArr = relativeUrl.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathArr.Length < 4)
            {
                return request;
            }
            if (!pathArr[0].EqualsOrNullEmpty(SiteConstants.ThumbnailRootPathName, StringComparison.OrdinalIgnoreCase))
            {
                return request;
            }
            if (!pathArr[1].IsAsciiDigit() || !pathArr[2].IsAsciiDigit())
            {
                return request;
            }
            request.Width = Convert.ToInt32(pathArr[1]);

            request.Height = Convert.ToInt32(pathArr[2]);

            string pathOrGuid = pathArr[3];

            var questionIndex = pathOrGuid.IndexOf("?"); 
            if (questionIndex>-1)
            {
                pathOrGuid = pathOrGuid.Substring(0, questionIndex); 
            }
            
            Guid imgGuid;
            if (Guid.TryParse(pathOrGuid, out imgGuid))
            {
                request.ImageId = imgGuid;
            }
            else
            {
                var newPath = new List<string>();
                for (int i = 3; i < pathArr.Length; i++)
                {
                    newPath.Add(pathArr[i]);
                }
                request.Path = String.Concat("/", String.Join("/", newPath));
            }

            return request;
        }

        public static string GenerateThumbnailUrl(Guid imageid, int width, int height, Guid siteId)
        {
            string url = String.Format(SiteConstants.ThumbnailPathFormat, width, height) + imageid;
            if (siteId != default(Guid))
            {
                url = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(url, DataConstants.SiteId, siteId.ToString());
            }
            return url;
        }

        public static string GenerateThumbnailUrl(Guid imageid, int width, int height)
        {
            return GenerateThumbnailUrl(imageid, width, height, default(Guid));
        }

        public static string GenerateThumbnailUrl(string imagePath, int width, int height, Guid SiteId)
        {
            if (imagePath.StartsWith("/"))
            {
                imagePath = imagePath.Substring(1);
            }

            string thumbnailPath = String.Format(SiteConstants.ThumbnailPathFormat, width, height);

            string fullurl = UrlHelper.Combine(thumbnailPath, imagePath);

            if (SiteId != default(Guid))
            {
                fullurl = Lib.Helper.UrlHelper.AppendQueryString(fullurl, DataConstants.SiteId, SiteId.ToString());
            }

            return fullurl;
        }

        public static string GenerateThumbnailUrl(string imagePath, int width, int height)
        {
            return GenerateThumbnailUrl(imagePath, width, height, default(Guid));
        }


        /// <summary>
        ///  formate of thumbnail. 
        /// /thumbnail/100/200/guid
        /// /thumbnail/100/200/imagefolder/image/name.jpg.
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="RelativeUrl"></param>
        /// <returns></returns>
        public static Thumbnail GetThumbnail(SiteDb SiteDb, string RelativeUrl)
        {
            ThumbnailRequest request = ParseThumbnailRequest(RelativeUrl);

            Guid imageid = request.ImageId;

            if (request.ImageId == default(Guid))
            {
                if (string.IsNullOrEmpty(request.Path))
                {
                    //TODO should implement the not image icon
                }
                else
                {

                    string url = request.Path;
                    var index = url.IndexOf("?"); 
                    if (index >-1)
                    {
                        url = url.Substring(0, index); 
                    }
                    Image koobooimage = SiteDb.Images.GetByUrl(url);

                    if (koobooimage != null)
                    {
                        imageid = koobooimage.Id;
                    }
                }
            }

            return SiteDb.Thumbnails.GetThumbnail(imageid, request.Width, request.Height);

        }

    }


    /// <summary>
    /// The request of a thumbnail. 
    /// </summary>
    public class ThumbnailRequest
    {
        public int Height { get; set; }

        public int Width { get; set; }

        /// <summary>
        /// Image path. 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Image Guid. Either Path or ImageId, one of them will be available. 
        /// </summary>
        public Guid ImageId { get; set; }

    }
}


