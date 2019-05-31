//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menus
{ 
    // the top square navigation... 
  public  interface IHeaderMenu : ICmsMenu
    { 
       string BadgeIcon { get; set; }
    }
}
