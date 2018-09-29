//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
   public class PagedResult
    {
        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public Type ReturnType { get; set;}

        public List<Object> DataList { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }
}
