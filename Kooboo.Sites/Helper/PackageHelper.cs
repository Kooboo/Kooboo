//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;
using Kooboo.Data;

namespace Kooboo.Sites.Helper
{
    public class PackageHelper
    {
        public static string GetExtension(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return ".png";
            }
            string ext = Kooboo.Lib.Helper.UrlHelper.FileExtension(input);

            if (string.IsNullOrEmpty(ext))
            {
                return ".png";
            }
            else
            {
                return ext;
            }
        }
        public static string GetThumbnailUrl(string thumbNail)
        {
            string imgbase = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/themeimg/");

            if (!string.IsNullOrEmpty(thumbNail) && !thumbNail.ToLower().StartsWith("http://"))
            {
                thumbNail = UrlHelper.Combine(imgbase, thumbNail);
            }
            thumbNail += "?width=200";

            return thumbNail;
        }

        public static List<string> GetImageDownloadUrl(List<string> images)
        {
            string imgbase = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/themeimg/");

            int count = images.Count();
            var newImages = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var current = images[i];
                var newurl = UrlHelper.Combine(imgbase, current);
                newImages.Add(newurl);
            }
            return newImages;
        }

        public static bool ContainSeach(string input, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return true;
            }

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}
