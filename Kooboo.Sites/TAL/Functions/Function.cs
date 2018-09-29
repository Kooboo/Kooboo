//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.TAL.Functions
{

    /// <summary>
    /// functions that can be used on tal:replace or tal:attributes.
    /// </summary>
  public  class Function
    { 
      public Function()
      {
          ParameterValues = new List<object>();
      } 

      public string FunctionName;

      public List<object> ParameterValues; 
    }
}
