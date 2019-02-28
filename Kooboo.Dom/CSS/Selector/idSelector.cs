//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS
{
  public   class idSelector : simpleSelector
    {
      public idSelector()
      {
          base.Type = enumSimpleSelectorType.id;
          
      }

      public string elementE;

      public string id;
    }
}
