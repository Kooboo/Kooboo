using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class KConfig : SiteObject
    {

        public KConfig()
        {
            this.ConstType = ConstObjectType.Code; 
        }

        public string Key { get; set; }

       


    }
}
