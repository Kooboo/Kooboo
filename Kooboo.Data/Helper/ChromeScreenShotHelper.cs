using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
                base64Image = GetThumbnailImage(base64Image, new Size() {Width = width, Height = height});
                // should verify as base64 here.. 
                return base64Image;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        //temp method will be remove 
        public static string GetThumbnailImage(string base64Str, Size size)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Str);
            MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(memoryStream, false);
            
            memoryStream.Close();

            using (MemoryStream ms = new MemoryStream())
            {
                size = GetEqualProportionSize(image, size);
                Image thumbImage = image.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero);
                //generate thumbImage
                thumbImage.Save(ms, ImageFormat.Png);
                var length = Convert.ToInt32(ms.Length);
                byte[] data = new byte[length];
                ms.Position = 0;
                ms.Read(data, 0, length);
                ms.Flush();
                return Convert.ToBase64String(data);
            }
        }

        public static Size GetEqualProportionSize(Image sourceImage,Size size)
        {
            int width;
            int height;
            //宽度比目的图片宽度大，长度比目的图片长度小
            if (sourceImage.Width > size.Width && sourceImage.Height <= size.Height)  
            {
                width = size.Width;
                height = (width * sourceImage.Height) / sourceImage.Width;
            }
            //宽度比目的图片宽度小，长度比目的图片长度大  
            else if (sourceImage.Width <= size.Width && sourceImage.Height > size.Height)
            {
                height = size.Height;
                width = (height * sourceImage.Width) / sourceImage.Height;
            }
            //长宽比目的图片长宽都小  
            else if (sourceImage.Width <= size.Width && sourceImage.Height <= size.Height) 
            {
                width = sourceImage.Width;
                height = sourceImage.Height;
            }
            //长宽比目的图片的长宽都大  
            else
            {
                width = size.Width;
                height = (width * sourceImage.Height) / sourceImage.Width;
                if (height > size.Height)//重新计算  
                {
                    height = size.Height;
                    width = (height * sourceImage.Width) / sourceImage.Height;
                }
            }
            return new Size()
            {
                Height = height,
                Width = width
            };
        }

    }
    
}
