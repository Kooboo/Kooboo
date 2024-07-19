//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Lib.Utilities
{
    public class CalculateUtility
    {
        /// <summary>
        /// Convert bytes to KB/MB/GB
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetSizeString(long bytes)
        {
            Decimal filesize = new Decimal(bytes);
            Decimal gigabytes = new Decimal(1024 * 1024 * 1024);
            var returnValue = filesize / gigabytes;
            if (returnValue > 1)
            {
                return (returnValue.ToString("#0.00") + "GB");
            }
            Decimal megabyte = new Decimal(1024 * 1024);
            returnValue = filesize / megabyte;
            if (returnValue > 1)
            {
                return (returnValue.ToString("#0.00") + "MB");
            }
            Decimal kilobyte = new Decimal(1024);
            returnValue = filesize / kilobyte;
            return (returnValue.ToString("#0.00") + "KB");
        }

        public static SizeMeansurement GetImageSize(byte[] imagebytes)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetImageSize(imagebytes);
        }
    }

    public class SizeMeansurement
    {
        public int Height { get; set; }

        public int Width { get; set; }
    }



}
