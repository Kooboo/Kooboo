using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Scripting.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Extension
{
    public class SiteUser : IkScript
    {
        [Attributes.SummaryIgnore]
        public string Name
        {
            get
            {
                return "SampleUser";
            }
        }

        [Attributes.SummaryIgnore]
        public RenderContext context { get; set; }

        public bool validate(string UserName, string Password)
        {
            return true; 
        } 

        public string get(string username)
        {
            return username + " sample"; 
        }

    }
}
