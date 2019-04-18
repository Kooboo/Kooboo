using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Server
{
    public interface IWebServer
    {
        void SetMiddleWares(List<IKoobooMiddleWare> middlewares);

        void Start();

        void Stop(); 
    }
}
