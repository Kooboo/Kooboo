//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Web.ViewModel
{
    public class LinkViewModel
    {
        public List<LinkItem> Pages
        { get; set; } = new List<LinkItem>();

        public List<LinkItem> Views
        { get; set; } = new List<LinkItem>();
    }

    public class LinkItem
    {
        public string Url { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
    }
}