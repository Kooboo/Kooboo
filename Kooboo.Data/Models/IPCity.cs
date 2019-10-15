//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Models
{
    public class IPCity
    {
        public string StrBegin { get; set; }
        public string StrEnd { get; set; }
        public int Begin { get; set; } = int.MinValue;

        public int End { get; set; } = int.MinValue;

        public string Country { get; set; }

        public string State { get; set; }
    }
}