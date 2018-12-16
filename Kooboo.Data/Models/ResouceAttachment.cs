//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class ResouceAttachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
    }
}
