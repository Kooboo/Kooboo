//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.rawmodel
{

    /// <summary>
    ///  A component value is one of the preserved tokens, a function, or a simple block.
    /// </summary>
  public  class ComponentValue
    {

      public CompoenentValueType Type;

      /// <summary>
     ///  the start position on the underlining css text string.
      /// </summary>
      public int startindex;

      /// <summary>
      /// the end position on the underlining css text string.
      /// </summary>
      public int endindex;
    }

  public enum CompoenentValueType
  {
      preservedToken =0,
      function =1,
      simpleBlock =2
  }

   

}
