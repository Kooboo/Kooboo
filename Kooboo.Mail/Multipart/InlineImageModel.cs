//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Multipart
{
    public class InlineImageModel
    {
        public string FileName { get; set; }

        public byte[] Binary { get; set; }

        public string ContentId { get; set; }
    }
}
