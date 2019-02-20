using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.temp
{
    public class AnotherSetting : Kooboo.Data.Interface.ISiteSetting
    {
        public string Name => "MyFirstName";

        public string keys { get; set; }
    }
}
