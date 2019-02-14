using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Backend.Menu
{
  public  interface ITopMenu : IMenu
    { 
       string BadgeIcon { get; set; }
    }
}
