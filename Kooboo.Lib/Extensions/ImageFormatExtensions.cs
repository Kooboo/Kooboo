//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Extensions
{
    public static class ImageFormatExtensions
    {
        public static ImageFormat GetImageFormat(this ImageCodecInfo imageInfo)
        {
            if (imageInfo == null || String.IsNullOrEmpty(imageInfo.MimeType))
            {
                return ImageFormat.Jpeg;
            }

            switch (imageInfo.MimeType.ToLower())
            {
                case "image/bmp":
                    return ImageFormat.Bmp;
                case "image/gif":
                    return ImageFormat.Gif;
                case "image/x-emf":
                    return ImageFormat.Emf;
                case "image/x-wmf":
                    return ImageFormat.Wmf;
                case "image/tiff":
                    return ImageFormat.Tiff;
                case "image/png":
                    return ImageFormat.Png;
                case "image/x-icon":
                    return ImageFormat.Icon;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        public static ImageCodecInfo GetImageCodecInfo(this string filePath)
        {
            filePath = Path.GetExtension(filePath);
            if (String.IsNullOrEmpty(filePath))
            {
                filePath = ".jpg";
            }
            var tmp = "*" + filePath;
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(it => it.FilenameExtension.Split(';').Contains(tmp, StringComparer.OrdinalIgnoreCase));
        }

        public static ImageFormat GetImageFormat(this string filePath)
        {
            return GetImageFormat(GetImageCodecInfo(filePath));
        }

        public static ImageFormat GetImageFormatByExtension(this string extension)
        {
            if (String.IsNullOrWhiteSpace(extension))
            {
                return ImageFormat.Jpeg;
            }
            if (!extension.Contains('.'))
            {
                extension = String.Concat(".", extension);
            }
            var tmp = String.Concat("*", Path.GetExtension(extension));
            var codecInfo = ImageCodecInfo.GetImageDecoders().FirstOrDefault(it => it.FilenameExtension.Split(';').Contains(tmp, StringComparer.OrdinalIgnoreCase));
            return codecInfo.GetImageFormat();
        }
    }
}
