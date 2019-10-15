using System.Collections.Generic;

namespace Kooboo.Data.Server
{
    public interface IWebServer
    {
        void SetMiddleWares(List<IKoobooMiddleWare> middlewares);

        void Start();

        void Stop(); 
    }
}
