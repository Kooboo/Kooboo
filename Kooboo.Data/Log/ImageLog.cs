//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
  public  class ImageLog
    {
        public Guid ImageId { get; set; }

        public string Url { get; set; }

        public int Size { get; set; }

        public string ClientIP { get; set; }

        public DateTime StartTime { get; set; } = new DateTime(); 
    }
}
