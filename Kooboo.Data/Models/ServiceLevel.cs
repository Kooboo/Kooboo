//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class ServiceLevel
    { 
        // The level of organization, start with 0. 
        public int Level { get; set; }

        public Quote Email { get; set; }

        public Quote Disk { get; set; }

        public Quote BandWidth { get; set; }

        public int MaxSites { get; set; } = 5;       
        // per site. 
        public int MaxPages { get; set; } = 100;

        public int MaxSsl { get; set; } = 4; 

    }

    public class Quote

    {
        public long Count { get; set; }

        public Frequence Frequence { get; set; }

    }

    public enum Frequence
    {
        Day = 0,
        Week = 2,
        Month = 3,
        Year = 4
    }

}
