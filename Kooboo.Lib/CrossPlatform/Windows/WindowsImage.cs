//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Lib.CrossPlatform.Windows
{
    public class WindowsImage : IImage
    {
        public int Priority => 2;

        public Size GetImageSize(byte[] imagebytes)
        {
            return new Size() { Height = 1, Width = 1 };
        }

        public byte[] GetThumbnailImage(byte[] contentBytes, int width, int height)
        {
            return null;
        }
    }
}