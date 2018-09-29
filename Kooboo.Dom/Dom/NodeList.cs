using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom
{
    [Serializable]
  public  class NodeList
    {

      public List<Node> item = new List<Node>();

      public int length
      {
          get {  
              return item.Count(); 
          }
      }

    }
}
