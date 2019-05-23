using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Menus.ObjectMenu
{
  public  class ObjectListMenu
    { 
        public string Name { get; set; }
         
        public string Url {
            get
            {
                // TODO: generate the URL. 
                return null; 
            }
        } 

        public Dictionary<string, string> Para { get; set; }

        public bool AcceptMultipleSelection { get; set; } = false; 

        public bool AcceptionSingleSelection { get; set; } = true; 
    }
}
