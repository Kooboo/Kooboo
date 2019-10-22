//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Mail.Models
{
    public class Attachment
    {
        public string FileName { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public long Size { get; set; }
    }
}