//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Kooboo.Data.Helper
{
 public static   class ChromeScreenShotHelper
    { 
        public static List<string> GetScreenShots(List<string> urls)
        {
            List<string> screenImages = new List<string>();
            foreach (var url in urls)
            {
                var screenshotImage = GetScreenShot(url);
                screenImages.Add(screenshotImage);
            }
            return screenImages;
        }
        public static string GetScreenShot(string url, int width = 600, int height =450)
        {
            url = System.Net.WebUtility.UrlEncode(url);
            var nodeScreenShotUrl = string.Format("{0}?url={1}&width={2}&height={3}", Data.AppSettings.ScreenShotUrl, url, width, height);
            try
            {
                var base64Image = HttpHelper.Get<string>(nodeScreenShotUrl);
                base64Image = GetThumbnailImage(base64Image, new ImageSize() {Width = width, Height = height});
                // should verify as base64 here.. 
                return base64Image;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        //temp method will be remove 
        public static string GetThumbnailImage(string base64Str, ImageSize size)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(base64Str, size);
        }


    }
    
}
