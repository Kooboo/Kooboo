//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class MyViewModel
    {
        public Dictionary<string, string> Title { get; set; }

        public List<MyListItemViewModel> List { get; set; }
    }

    public class MyListItemViewModel
    {
        public string Title { get; set; }

        public string Id { get; set; }
         
        public string Url { get; set; }
    }
}
