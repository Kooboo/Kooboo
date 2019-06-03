//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menus
{
  public interface ISideBarMenu : ICmsMenu
    {
       EnumSideBarParent Parent { get; set; }
    }

    public enum EnumSideBarParent
    {
        Root = 0,
        Feature=1, 
        System = 2,
        Development = 3,
        Contents = 4,
        Database =5
    }
}
