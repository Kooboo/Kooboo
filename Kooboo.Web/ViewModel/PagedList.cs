//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
