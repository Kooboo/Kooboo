using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Kooboo.Lib.Utilities;
using System.Drawing;
#if NETSTANDARD
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
#endif

namespace Kooboo.Lib.Helper
{
    public class SystemImageHelper
    {

        public static SizeMeansurement GetImageSize(byte[] imagebytes)
        {
            SizeMeansurement measure = new SizeMeansurement();

            try
            {
                MemoryStream stream = new MemoryStream(imagebytes);
#if NETSTANDARD
                var image = Image.Identify(stream);
                measure.Height = image.Height;
                measure.Width = image.Width;
#else
                System.Drawing.Image image = null;
                image = System.Drawing.Image.FromStream(stream);
                measure.Height = image.Height;
                measure.Width = image.Width;
#endif
            }
            catch (Exception ex)
            {

            }
            return measure;
        }

        public static byte[] GetThumbnailImage(byte[] contentBytes,int width,int height)
        {
            if (contentBytes == null) return null;

            MemoryStream stream = new MemoryStream(contentBytes);

            
#if NETSTANDARD
            Image<SixLabors.ImageSharp.PixelFormats.Rgba32> systhumbnail = null;
            Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image = Image.Load(stream);
            if (image.Width < width && image.Height < height)
            {
                return contentBytes;
            }

            image.Mutate(x => x.Resize(width, height));
            systhumbnail = image;

            MemoryStream memstream = new MemoryStream();
            systhumbnail.Save(memstream, ImageFormats.Png);
#else
            System.Drawing.Image image = null;
            System.Drawing.Image systhumbnail = null;

            image = System.Drawing.Image.FromStream(stream);

            if (image.Width < width && image.Height < height)
            {
                return contentBytes;
            }

            systhumbnail = image.GetThumbnailImage(width, height, null, new IntPtr());

            MemoryStream memstream = new MemoryStream();
            systhumbnail.Save(memstream, System.Drawing.Imaging.ImageFormat.Png);
#endif
            return memstream.ToArray();
        }

        public static string GetThumbnailImage(string base64Str, Size size)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Str);

            MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
#if NETSTANDARD
            var image = Image.Load(memoryStream);
#else
            Image image = Image.FromStream(memoryStream, false);
#endif

            memoryStream.Close();

            using (MemoryStream ms = new MemoryStream())
            {
                size = GetEqualProportionSize(image.Width,image.Height, size);
#if NETSTANDARD
                image.Mutate(x => x.Resize(size.Width, size.Height));
                image.Save(ms, ImageFormats.Png);
#else
                Image thumbImage = image.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero);
                //generate thumbImage
                thumbImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
#endif
                var length = Convert.ToInt32(ms.Length);
                byte[] data = new byte[length];
                ms.Position = 0;
                ms.Read(data, 0, length);
                ms.Flush();
                return Convert.ToBase64String(data);
            }
        }

        private static Size GetEqualProportionSize(int sourceWidth,int sourceHeight, Size size)
        {
            int width;
            int height;
            //宽度比目的图片宽度大，长度比目的图片长度小
            if (sourceWidth > size.Width && sourceHeight <= size.Height)
            {
                width = size.Width;
                height = (width * sourceHeight) / sourceWidth;
            }
            //宽度比目的图片宽度小，长度比目的图片长度大  
            else if (sourceWidth <= size.Width && sourceHeight > size.Height)
            {
                height = size.Height;
                width = (height * sourceWidth) / sourceHeight;
            }
            //长宽比目的图片长宽都小  
            else if (sourceWidth <= size.Width && sourceHeight <= size.Height)
            {
                width = sourceWidth;
                height = sourceHeight;
            }
            //长宽比目的图片的长宽都大  
            else
            {
                width = size.Width;
                height = (width * sourceHeight) / sourceWidth;
                if (height > size.Height)//重新计算  
                {
                    height = size.Height;
                    width = (height * sourceWidth) / sourceHeight;
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
