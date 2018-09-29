//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
  public  class DashBoardResponseModel : IDashBoardResponse
    {
        public object Model { get; set; }

        public string ViewName { get; set; }

        public string ViewBody { get; set; }

        public string Link { get; set; }
       
    }
}
