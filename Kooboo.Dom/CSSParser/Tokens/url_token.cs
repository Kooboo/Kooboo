//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.Tokens
{
  public  class url_token : cssToken
    {

      public url_token()
      {
          this.Type = enumTokenType.url;

      }

      public string value;

      /// <summary>
      /// Url does not seems to be used, url at the value property.
      /// </summary>
      public string url;
    }
}
