using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class KConfig : CoreObject
    {      
        public KConfig()
        {
            this.ConstType = ConstObjectType.Kconfig; 
        }
               
        public string Binding { get; set; }

        // The original tag.
        public string TagName { get; set; }

        public string TagHtml { get; set; }

        public override int GetHashCode()
        {
            string unique = this.TagName + this.TagHtml + this.Binding;   
            return Lib.Security.Hash.ComputeInt(unique);  
        }

    }
}
