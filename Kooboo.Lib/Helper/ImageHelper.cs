//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

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
    }

    public class ImageSize
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }

}
