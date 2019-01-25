//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Data.Interface; 
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
  public  class DashBoardResponseHtml: IDashBoardResponse
    {
        public string Body { get; set; }

      
         public string Link { get; set; }
    }
}
