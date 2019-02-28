//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class TransferStatusItemViewModel
    {
        public string Url { get; set; }
        public bool Done { get; set; }
    }

    public class TransferTaskStatusViewModel
    {
        public bool Done { get; set; }

        public List<TransferStatusItemViewModel> UrlStatus { get; set; }
    }
}
