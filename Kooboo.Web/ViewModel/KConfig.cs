using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{    
    public class KConfigItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Binding { get; set; }
                                               
        public string TagName { get; set; }

        public string TagHtml { get; set; }

        public DateTime LastModified
        {
            get; set;
        }
        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public Dictionary<string, int> Relations { get; set; }

    }
           


}
