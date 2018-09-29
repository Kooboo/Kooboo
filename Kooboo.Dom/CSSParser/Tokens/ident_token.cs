using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.Tokens
{
  public  class ident_token : cssToken
    {

      public ident_token()
      {

          this.Type = enumTokenType.ident;
      }

      public string value;

    }
}
