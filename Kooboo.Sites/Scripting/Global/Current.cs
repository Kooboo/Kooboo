//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global
{
  public class Current
    { 
        public Kooboo.Sites.Models.Page Page { get; set; }

        public Models.View View { get; set; }

        public Models.Layout Layout { get; set; } 
    }
}
