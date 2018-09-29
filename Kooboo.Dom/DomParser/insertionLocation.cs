//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom
{
  public  class insertionLocation
    {
      public Element partentElement;

      /// <summary>
      /// The location to insert the new element. 
      /// -1 = append to the last child. 
      /// </summary>
      public int insertAt = -1;

    }
}
