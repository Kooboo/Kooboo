//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;

namespace Kooboo.Lib.Helper
{
    public class ImageHelper
    {
        public static ImageSize GetEqualProportionSize(int sourceWidth, int sourceHeight, ImageSize size)
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
            return new ImageSize()
            {
                Height = height,
                Width = width
            };
        }

        public static int GetGifFrameCount(byte[] image)
        {
            if (image == null || image.Length == 0)
            {
                return 0;
            }

            try
            {
                using MemoryStream mo = new MemoryStream(image);
                using var gif = Image.Load(mo);
                return gif.Frames.Count;
            }
            catch (Exception)
            {
            }

            return 0;
        }

        public static byte[] ConvertToTwoFramesGif(byte[] image)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            try
            {
                using var mo = new MemoryStream(image);
                using var gif = Image.Load(mo);

                if (gif.Frames.Count < 2)
                {
                    var firstFrame = gif.Frames.CloneFrame(0);
                    gif.Frames.AddFrame(firstFrame.Frames.First());
                    using var resultMemoryStream = new MemoryStream();
                    gif.SaveAsGif(resultMemoryStream);
                    return resultMemoryStream.ToArray();
                }
                else
                {
                    return image;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static string GetFileExtension(byte[] image)
        {
            if (image == null)
            {
                return null;
            }

            MemoryStream mo = new MemoryStream(image);
            try
            {
                var formate = Image.DetectFormat(mo);
                if (formate != null && formate.FileExtensions != null && formate.FileExtensions.Any())
                {
                    return formate.FileExtensions.FirstOrDefault();
                }


            }
            catch (Exception)
            {

            }

            return null;
        }
    }

    public class ImageSize
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }

}
