using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Server
{
   public static  class WebServerFactory
    { 
        public static IWebServer Create(int port, List<IKoobooMiddleWare> MiddleWares, bool forceSSL=false)
        {
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

            // Below for .NET core...  
 
        }
    }
}
