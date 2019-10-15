//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

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
