//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Web.ViewModel
{
    public class PagedListViewModel<T>
    {
        public List<T> List { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageNr { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}