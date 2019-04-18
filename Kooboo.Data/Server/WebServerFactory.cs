using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Server
{
   public static  class WebServerFactory
    { 
        public static IWebServer Create(int port, List<IKoobooMiddleWare> MiddleWares, bool forceSSL=false)
        {
#if NETSTANDARD2_0

            KestrelWebServer server = new KestrelWebServer(port, MiddleWares, forceSSL);
            server.SetMiddleWares(MiddleWares);
            return server; 
#else

            if (forceSSL)
            {
                WebServer server = new WebServer(port, true);
                server.SetMiddleWares(MiddleWares);
                return server;
            }
            else
            {
                WebServer server = new WebServer(port);
                server.SetMiddleWares(MiddleWares);
                return server;
            } 

#endif 
 
        }
    }
}
