//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Converter
{
    public class ConvertContext
    {
        public SiteDb SiteDb { get; set; }

        public Guid PageId { get; set; }

        private Page _page;
        public Page Page
        {
            get
            {
                if (_page == null && SiteDb != null)
                {
                    return SiteDb.Pages.Get(PageId);
                }
                return _page;
            }
            set
            {
                _page = value; 
            }
        }

        public string Culture { get; set; } 
    }
}
