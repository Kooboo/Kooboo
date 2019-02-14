using Kooboo.Api;
using Kooboo.Data.Context; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Backend.Menu
{
    public interface IMenu
    {
        string Name { get; set; }
         
        string DisplayName(RenderContext Context); 

        string Icon { get; set; }

        string Url { get; set; }

        // Does not show any more.  
        bool Disable { get; set; }
         
        List<IMenu> Items
        {
            get; set;
        }  
    }
}
