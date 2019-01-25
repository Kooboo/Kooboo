//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class IPCity
    {
        public string strBegin { get; set; }
        public string StrEnd { get; set; }
        public int Begin { get; set; } = int.MinValue;

        public int End { get; set; } = int.MinValue;

        public string Country { get; set; }

        public string State { get; set; }
    }
}
