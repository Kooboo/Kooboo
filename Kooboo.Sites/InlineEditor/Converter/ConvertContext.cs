//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;

namespace Kooboo.Sites.InlineEditor.Converter
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