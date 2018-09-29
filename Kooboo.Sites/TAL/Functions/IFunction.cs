//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.TAL
{
  public  interface IFunction
    {
      /// <summary>
      /// The unique name of this function that can used on the rendering. 
      /// e.g. format, DateTimeNow. 
      /// </summary>
      string Name { get; }

      string Description { get; }

      object Execute(params object[] paras);

    }
}
