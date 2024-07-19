//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;

namespace Kooboo.Render
{
    public class RenderRespnose
    {
        public UrlFileType FileType { get; set; }

        public string ContentType { get; set; }

        public byte[] BinaryBytes { get; set; }

        public string Body { get; set; }

        public Stream Stream { get; set; }
    }
}
