//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Multipart
{
    public class InlineImageModel
    {
        public string FileName { get; set; }

        public byte[] Binary { get; set; }

        public string ContentId { get; set; } 
    }
}
