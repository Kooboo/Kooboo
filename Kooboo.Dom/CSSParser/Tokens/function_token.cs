//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.Tokens
{
   public class function_token : cssToken
    {
       public function_token()
       {
           this.Type = enumTokenType.function;
       }

  
       public string Value;

    }

   
}
