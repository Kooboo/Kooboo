using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;

namespace Kooboo.Data.Server
{
    public class EndMiddleWare : IKoobooMiddleWare
    {
        public IKoobooMiddleWare Next
        {
            get;set;
        }

        public async Task Invoke(RenderContext context)
        {
            return; 
        }
    }
}
