//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class DataCenter
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; } = 80; 

        [Obsolete]
        public bool IsCustom { get; set; }  // only local server can custom now...

        public bool IsSelected { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsRoot { get; set; }

        public string PrimaryDomain { get; set; }


    }
}
